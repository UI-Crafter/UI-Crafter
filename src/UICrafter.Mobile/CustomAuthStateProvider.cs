namespace UICrafter.Mobile;

using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Identity.Client;
using Serilog;
using UICrafter.Mobile.MSALClient;

public class CustomAuthStateProvider : AuthenticationStateProvider, IDisposable
{
	private readonly HttpClient _httpClient;
	private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

	private readonly PeriodicTimer _periodicTimer = new(TimeSpan.FromMinutes(5));

	public CustomAuthStateProvider(HttpClient httpClient)
	{
		_httpClient = httpClient;
		InitializeAuthenticationStateAsync();

		Task.Run(RunTimer);
	}

	public override Task<AuthenticationState> GetAuthenticationStateAsync() => Task.FromResult(new AuthenticationState(_currentUser));

	private async void InitializeAuthenticationStateAsync()
	{
		try
		{
			// Attempt to acquire a cached token silently
			var result = await PublicClientSingleton.Instance
				.MSALClientHelper
				.TrySignInUserSilently(PublicClientSingleton.Instance.GetScopes()!)
				.ConfigureAwait(false);

			// Set the authenticated user if a cached token is available
			if (result != null)
			{
				_currentUser = CreateClaimsPrincipal(result);
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.IdToken);
			}
		}
		catch (MsalUiRequiredException)
		{
			// Remain in unauthenticated state if no cached token is available
			_currentUser = new ClaimsPrincipal(new ClaimsIdentity());
		}
		catch (Exception ex)
		{
			Log.Error("Failed to initialize authentication state", ex);
		}

		NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
	}

	public async Task LogInAsync()
	{
		var loginTask = LogInAsyncCore();
		NotifyAuthenticationStateChanged(loginTask);
		await loginTask;
	}

	private async Task<AuthenticationState> LogInAsyncCore()
	{
		try
		{
			// Acquire token interactively
			var result = await PublicClientSingleton.Instance
				.MSALClientHelper
				.SignInUserAndAcquireAccessToken(PublicClientSingleton.Instance.GetScopes()!);

			// Set user with claims from result
			_currentUser = CreateClaimsPrincipal(result!);
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result!.IdToken);

			var response = await _httpClient.PutAsync("user/authenticated", null);
			if (!response.IsSuccessStatusCode)
			{
				// Handle failed registration if necessary
				throw new InvalidOperationException("Backend login registration failed.");
			}
		}
		catch (Exception ex)
		{
			Log.Error("Error during interactive login", ex);
		}

		return new AuthenticationState(_currentUser);
	}

	public async Task SignOutAsync()
	{
		_currentUser = new ClaimsPrincipal(new ClaimsIdentity());
		await PublicClientSingleton.Instance.MSALClientHelper.SignOutUserAsync();
		_httpClient.DefaultRequestHeaders.Authorization = null;

		NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
	}

	private static ClaimsPrincipal CreateClaimsPrincipal(AuthenticationResult result)
	{
		var claims = new List<Claim> { new("AccessToken", result.IdToken) };
		claims.AddRange(result.ClaimsPrincipal.Claims);

		var identity = new ClaimsIdentity(claims, "Custom", "name", ClaimTypes.Role);
		return new ClaimsPrincipal(identity);
	}

	private async Task RunTimer()
	{
		while (await _periodicTimer.WaitForNextTickAsync())
		{
			if (_currentUser.Identity is not null && _currentUser.Identity.IsAuthenticated)
			{
				await RefreshTokenAsync();
			}
		}
	}

	private async Task RefreshTokenAsync()
	{
		try
		{
			// Try to acquire a new access token with force refresh
			var result = await PublicClientSingleton.Instance.MSALClientHelper.ForceRefreshAccessTokenAsync(PublicClientSingleton.Instance.GetScopes()!);

			if (result is not null)
			{
				_currentUser = CreateClaimsPrincipal(result);
				NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
			}
		}
		catch (MsalUiRequiredException)
		{
			_currentUser = new ClaimsPrincipal(new ClaimsIdentity());
			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
		}
		catch (Exception ex)
		{
			Log.Error($"Token refresh failed", ex);
		}
	}

	public void Dispose()
	{
		_periodicTimer.Dispose();
		GC.SuppressFinalize(this);
	}
}
