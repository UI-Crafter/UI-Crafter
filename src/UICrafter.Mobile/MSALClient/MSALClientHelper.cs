// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace UICrafter.Mobile.MSALClient;

using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using Serilog;

/// <summary>
/// Contains methods that initialize and use the MSAL SDK
/// </summary>
public class MSALClientHelper
{
	/// <summary>
	/// As for the Tenant, you can use a name as obtained from the azure portal, e.g. kko365.onmicrosoft.com"
	/// </summary>
	public AzureAdConfig AzureAdConfig { get; set; }

	/// <summary>
	/// Gets the authentication result (if available) from MSAL's various operations.
	/// </summary>
	/// <value>
	/// The authentication result.
	/// </value>
	public AuthenticationResult? AuthResult { get; private set; }

	/// <summary>
	/// Gets the MSAL public client application instance.
	/// </summary>
	/// <value>
	/// The public client application.
	/// </value>
	public IPublicClientApplication PublicClientApplication { get; private set; } = default!;

	/// <summary>
	/// This will determine if the Interactive Authentication should be Embedded or System view
	/// </summary>
	public bool UseEmbedded { get; set; }

	/// <summary>
	/// The PublicClientApplication builder used internally
	/// </summary>
	private readonly PublicClientApplicationBuilder _publicClientApplicationBuilder;

	// Token Caching setup - Mac
	public static readonly string KeyChainServiceName = "UICrafter.Auth";

	public static readonly string KeyChainAccountName = "MSALCache";

	public static readonly KeyValuePair<string, string> LinuxKeyRingAttr2 = new("ProductGroup", "UICrafter");

	private static readonly string _pCANotInitializedExceptionMessage = "The PublicClientApplication needs to be initialized before calling this method. Use InitializePublicClientAppAsync() to initialize.";

	/// <summary>
	/// Initializes a new instance of the <see cref="MSALClientHelper"/> class.
	/// </summary>
	public MSALClientHelper(AzureAdConfig azureAdConfig)
	{
		AzureAdConfig = azureAdConfig;

		_publicClientApplicationBuilder = PublicClientApplicationBuilder.Create(AzureAdConfig.ClientId)
			.WithExperimentalFeatures() // this is for upcoming logger
			.WithAuthority(AzureAdConfig.Authority)
			.WithLogging(new MyIdentityLogger(), enablePiiLogging: false)
			.WithIosKeychainSecurityGroup("com.uicrafter.adalcache");
	}

	/// <summary>
	/// Initializes the public client application of MSAL.NET with the required information to correctly authenticate the user.
	/// </summary>
	/// <returns></returns>
	public async Task<IAccount?> InitializePublicClientAppAsync()
	{
		// Initialize the MSAL library by building a public client application
		PublicClientApplication = _publicClientApplicationBuilder
#if WINDOWS
			.WithRedirectUri("http://localhost")
#else
			.WithRedirectUri($"msal{PublicClientSingleton.Instance.MSALClientHelper.AzureAdConfig.ClientId}://auth")
#endif
			.Build();

		await AttachTokenCache();
		return await FetchSignedInUserFromCache().ConfigureAwait(false);
	}

	/// <summary>
	/// Attaches the token cache to the Public Client app.
	/// </summary>
	/// <returns>IAccount list of already signed-in users (if available)</returns>
	private async Task<IEnumerable<IAccount>?> AttachTokenCache()
	{
		if (DeviceInfo.Current.Platform != DevicePlatform.WinUI)
		{
			return null;
		}

		// Cache configuration and hook-up to public application. Refer to https://github.com/AzureAD/microsoft-authentication-extensions-for-dotnet/wiki/Cross-platform-Token-Cache#configuring-the-token-cache
		var storageProperties = new StorageCreationPropertiesBuilder(AzureAdConfig.CacheFileName, AzureAdConfig.CacheDir)
				.Build();

		var msalcachehelper = await MsalCacheHelper.CreateAsync(storageProperties);
		msalcachehelper.RegisterCache(PublicClientApplication.UserTokenCache);

		// If the cache file is being reused, we'd find some already-signed-in accounts
		return await PublicClientApplication.GetAccountsAsync().ConfigureAwait(false);
	}

	/// <summary>
	/// Signs in the user and obtains an Access token for a provided set of scopes
	/// </summary>
	/// <param name="scopes"></param>
	/// <returns> Access Token</returns>
	public async Task<string> SignInUserAndAcquireAccessToken(string[] scopes)
	{
		Exception<NullReferenceException>.ThrowOn(() => PublicClientApplication is null, _pCANotInitializedExceptionMessage);

		var existingUser = await FetchSignedInUserFromCache().ConfigureAwait(false);

		try
		{
			// 1. Try to sign-in the previously signed-in account
			if (existingUser is not null)
			{
				AuthResult = await PublicClientApplication
					.AcquireTokenSilent(scopes, existingUser)
					.ExecuteAsync()
					.ConfigureAwait(false);
			}
			else
			{
				AuthResult = await SignInUserInteractivelyAsync(scopes);
			}
		}
		catch (MsalUiRequiredException ex)
		{
			Log.Information("A MsalUiRequiredException happened on AcquireTokenSilentAsync. This indicates you need to call AcquireTokenInteractive to acquire a token interactively", ex);

			AuthResult = await PublicClientApplication
				.AcquireTokenInteractive(scopes)
				//.WithUseEmbeddedWebView(true)
				.ExecuteAsync()
				.ConfigureAwait(false);
		}
		catch (MsalException msalEx)
		{
			Log.Error("Error Acquiring Token interactively", msalEx);
		}

		return AuthResult?.IdToken ?? string.Empty;
	}

	/// <summary>
	/// Shows a pattern to sign-in a user interactively in applications that are input constrained and would need to fall-back on device code flow.
	/// </summary>
	/// <param name="scopes">The scopes.</param>
	/// <param name="existingAccount">The existing account.</param>
	/// <returns></returns>
	public async Task<AuthenticationResult> SignInUserInteractivelyAsync(string[] scopes, IAccount? existingAccount = null)
	{

		Exception<NullReferenceException>.ThrowOn(() => PublicClientApplication == null, _pCANotInitializedExceptionMessage);

		if (PublicClientApplication.IsUserInteractive())
		{
			return await PublicClientApplication.AcquireTokenInteractive(scopes)
				//.WithUseEmbeddedWebView(true)
				.WithParentActivityOrWindow(PlatformConfig.Instance.ParentWindow)
				.ExecuteAsync()
				.ConfigureAwait(false);
		}

		// If the operating system does not have UI (e.g. SSH into Linux), you can fallback to device code, however this
		// flow will not satisfy the "device is managed" CA policy.
		return await PublicClientApplication.AcquireTokenWithDeviceCode(scopes, (dcr) =>
		{
			Log.Information(dcr.Message);
			return Task.CompletedTask;
		}).ExecuteAsync().ConfigureAwait(false);
	}

	/// <summary>
	/// Removes the first signed-in user's record from token cache
	/// </summary>
	public async Task SignOutUserAsync()
	{
		var existingUser = await FetchSignedInUserFromCache().ConfigureAwait(false);
		await SignOutUserAsync(existingUser).ConfigureAwait(false);
	}

	/// <summary>
	/// Removes a given user's record from token cache
	/// </summary>
	/// <param name="user">The user.</param>
	public async Task SignOutUserAsync(IAccount user)
	{
		if (PublicClientApplication is null)
		{
			return;
		}

		await PublicClientApplication.RemoveAsync(user).ConfigureAwait(false);
	}

	/// <summary>
	/// Fetches the signed in user from MSAL's token cache (if available).
	/// </summary>
	/// <returns></returns>
	public async Task<IAccount?> FetchSignedInUserFromCache()
	{
		Exception<NullReferenceException>.ThrowOn(() => PublicClientApplication == null, _pCANotInitializedExceptionMessage);

		var accounts = await PublicClientApplication.GetAccountsAsync();

		// Error corner case: we should always have 0 or 1 accounts, not expecting > 1
		// This is just an example of how to resolve this ambiguity, which can arise if more apps share a token cache.
		// Note that some apps prefer to use a random account from the cache.
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
