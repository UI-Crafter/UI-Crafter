namespace UICrafter.Mobile;

using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
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

	public async Task LogInAsync()
	{
		var loginTask = LogInAsyncCore();
		NotifyAuthenticationStateChanged(loginTask);
		await loginTask;
	}

	private async Task<AuthenticationState> LogInAsyncCore()
	{
		var user = await LoginWithPublicClientSingletonAsync();
		_currentUser = user;
		return new AuthenticationState(_currentUser);
	}

	private async Task<ClaimsPrincipal> LoginWithPublicClientSingletonAsync()
	{
		// Set whether the interactive login should use the embedded or system browser
		PublicClientSingleton.Instance.UseEmbedded = true;

		// Acquire token interactively using PublicClientSingleton
		var result = await PublicClientSingleton.Instance.AcquireTokenInteractiveAsync(PublicClientSingleton.Instance.GetScopes()!);

		var claims = new List<Claim> { new Claim("AccessToken", result.IdToken) };
		claims.AddRange(result.ClaimsPrincipal.Claims);

		var identity = new ClaimsIdentity(claims, "Custom", "name", ClaimTypes.Role);
		var principal = new ClaimsPrincipal(identity);

		// Set the access token in the HTTP client headers
		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.IdToken);

		var response = await _httpClient.PutAsync("user/authenticated", null);
		if (!response.IsSuccessStatusCode)
		{
			// Handle failed registration if necessary
			throw new InvalidOperationException("Backend login registration failed.");
		}

		return principal;
	}

	public void Logout()
	{
		_currentUser = new ClaimsPrincipal(new ClaimsIdentity());
		PublicClientSingleton.Instance.SignOutAsync(); // Clear user cache in singleton
		NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
	}

	private async void InitializeAuthenticationStateAsync()
	{
		// Check if there is a cached user in the singleton and initialize _currentUser accordingly
		var cachedUser = await PublicClientSingleton.Instance.AcquireTokenSilentAsync(PublicClientSingleton.Instance.GetScopes()!);
		if (cachedUser != null)
		{
			var claims = new List<Claim> { new Claim("AccessToken", cachedUser) };
			_currentUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "Custom", "name", ClaimTypes.Role));
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", cachedUser);
			NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
		}
	}
}
