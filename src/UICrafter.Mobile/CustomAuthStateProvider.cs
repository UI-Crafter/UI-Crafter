namespace UICrafter.Mobile;

using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Identity.Client;
using Serilog;
using UICrafter.Mobile.MSALClient;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
	private readonly HttpClient _httpClient;
	private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

	public CustomAuthStateProvider(HttpClient httpClient)
	{
		_httpClient = httpClient;
		InitializeAuthenticationStateAsync();
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
}
