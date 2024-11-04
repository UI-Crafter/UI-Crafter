namespace UICrafter.Mobile.MSALClient;

using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using Serilog;

/// <summary>
/// Provides methods to initialize and use the MSAL SDK for handling authentication and token caching.
/// </summary>
public class MSALClientHelper
{
	/// <summary>
	/// Configuration details for Azure AD, including the tenant and authority.
	/// </summary>
	public AzureAdConfig AzureAdConfig { get; set; }

	/// <summary>
	/// Stores the authentication result obtained from MSAL operations.
	/// </summary>
	public AuthenticationResult? AuthResult { get; private set; }

	/// <summary>
	/// The instance of MSAL's public client application used for authentication.
	/// </summary>
	public IPublicClientApplication PublicClientApplication { get; private set; } = default!;

	/// <summary>
	/// Builder used to configure the MSAL public client application.
	/// </summary>
	private readonly PublicClientApplicationBuilder _publicClientApplicationBuilder;

	// Token caching properties for macOS and Linux
	public static readonly string KeyChainServiceName = "UICrafter.Auth";
	public static readonly string KeyChainAccountName = "MSALCache";
	public static readonly KeyValuePair<string, string> LinuxKeyRingAttr2 = new("ProductGroup", "UICrafter");

	private static readonly string _pCANotInitializedExceptionMessage = "The PublicClientApplication needs to be initialized before calling this method. Use InitializePublicClientAppAsync() to initialize.";

	/// <summary>
	/// Initializes a new instance of <see cref="MSALClientHelper"/> with the specified Azure AD configuration.
	/// </summary>
	public MSALClientHelper(AzureAdConfig azureAdConfig)
	{
		AzureAdConfig = azureAdConfig;

		_publicClientApplicationBuilder = PublicClientApplicationBuilder.Create(AzureAdConfig.ClientId)
			.WithExperimentalFeatures()
			.WithAuthority(AzureAdConfig.Authority)
			.WithLogging(new MyIdentityLogger(), enablePiiLogging: false)
			.WithIosKeychainSecurityGroup("com.uicrafter.adalcache");
	}

	/// <summary>
	/// Initializes the MSAL public client application for user authentication.
	/// </summary>
	public async Task InitializePublicClientAppAsync()
	{
		// Configure the redirect URI based on platform
		PublicClientApplication = _publicClientApplicationBuilder
#if WINDOWS
			.WithRedirectUri("http://localhost")
#else
			.WithRedirectUri($"msal{PublicClientSingleton.Instance.MSALClientHelper.AzureAdConfig.ClientId}://auth")
#endif
			.Build();

		await AttachTokenCache();
	}

	/// <summary>
	/// Attaches the token cache to the MSAL client to enable secure storage and retrieval of tokens.
	/// </summary>
	/// <returns>Returns a list of accounts from the token cache, if available.</returns>
	private async Task<IEnumerable<IAccount>?> AttachTokenCache()
	{
		if (DeviceInfo.Current.Platform != DevicePlatform.WinUI)
		{
			return null;
		}

		var storageProperties = new StorageCreationPropertiesBuilder(AzureAdConfig.CacheFileName, AzureAdConfig.CacheDir).Build();
		var msalcachehelper = await MsalCacheHelper.CreateAsync(storageProperties);
		msalcachehelper.RegisterCache(PublicClientApplication.UserTokenCache);

		return await PublicClientApplication.GetAccountsAsync().ConfigureAwait(false);
	}

	/// <summary>
	/// Attempts to silently sign in the user using cached tokens if available.
	/// </summary>
	/// <param name="scopes">Scopes required for the token.</param>
	/// <returns>Authentication result if successful; otherwise, null.</returns>
	public async Task<AuthenticationResult?> TrySignInUserSilently(string[] scopes)
	{
		Exception<NullReferenceException>.ThrowOn(() => PublicClientApplication is null, _pCANotInitializedExceptionMessage);

		var existingUser = await FetchSignedInUserFromCache().ConfigureAwait(false);

		if (existingUser is null)
		{
			return null;
		}

		try
		{
			AuthResult = await PublicClientApplication
				.AcquireTokenSilent(scopes, existingUser)
				.ExecuteAsync();
		}
		catch (MsalException msalEx)
		{
			Log.Warning("Error acquiring token silently", msalEx);
		}

		return AuthResult;
	}

	/// <summary>
	/// Initiates the user sign-in process and obtains an access token for the specified scopes.
	/// </summary>
	/// <param name="scopes">The required access token scopes.</param>
	/// <returns>The authentication result containing the access token.</returns>
	public async Task<AuthenticationResult?> SignInUserAndAcquireAccessToken(string[] scopes)
	{
		Exception<NullReferenceException>.ThrowOn(() => PublicClientApplication is null, _pCANotInitializedExceptionMessage);

		var existingUser = await FetchSignedInUserFromCache().ConfigureAwait(false);

		try
		{
			AuthResult = existingUser is not null
				? await PublicClientApplication.AcquireTokenSilent(scopes, existingUser).ExecuteAsync()
				: await SignInUserInteractive(scopes);
		}
		catch (MsalUiRequiredException ex)
		{
			Log.Information("UI required for token acquisition. Attempting interactive sign-in.", ex);
			AuthResult = await SignInUserInteractive(scopes);
		}
		catch (MsalException msalEx)
		{
			Log.Error("Error acquiring token interactively", msalEx);
			throw;
		}

		return AuthResult;
	}

	/// <summary>
	/// Initiates an interactive sign-in to acquire an access token.
	/// </summary>
	/// <param name="scopes">The required access token scopes.</param>
	/// <returns>The authentication result containing the access token.</returns>
	public async Task<AuthenticationResult?> SignInUserInteractive(string[] scopes)
	{
		return AuthResult = await PublicClientApplication
				.AcquireTokenInteractive(scopes)
				.WithParentActivityOrWindow(PlatformConfig.Instance.ParentWindow)
				.ExecuteAsync()
				.ConfigureAwait(false);
	}

	/// <summary>
	/// Signs out the first signed-in user and removes their token cache.
	/// </summary>
	public async Task SignOutUserAsync()
	{
		var existingUser = await FetchSignedInUserFromCache().ConfigureAwait(false);
		await SignOutUserAsync(existingUser).ConfigureAwait(false);
	}

	/// <summary>
	/// Signs out a specified user by removing their record from the token cache.
	/// </summary>
	/// <param name="user">The user to sign out.</param>
	public async Task SignOutUserAsync(IAccount? user)
	{
		if (PublicClientApplication is null || user is null)
		{
			return;
		}

		await PublicClientApplication.RemoveAsync(user).ConfigureAwait(false);
	}

	/// <summary>
	/// Retrieves the signed-in user from the MSAL token cache, if available.
	/// </summary>
	/// <returns>The account of the signed-in user or null if no account is found.</returns>
	public async Task<IAccount?> FetchSignedInUserFromCache()
	{
		Exception<NullReferenceException>.ThrowOn(() => PublicClientApplication == null, _pCANotInitializedExceptionMessage);

		var accounts = await PublicClientApplication.GetAccountsAsync();

		if (accounts.Count() > 1)
		{
			foreach (var acc in accounts)
			{
				await PublicClientApplication.RemoveAsync(acc);
			}

			return null;
		}

		return accounts.SingleOrDefault();
	}
}
