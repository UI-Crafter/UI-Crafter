namespace UICrafter.Mobile;

using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Identity.Client;

// Inspired from this guide: https://medium.com/@ganeshonline6301/secure-your-net-maui-blazor-hybrid-app-with-azure-entra-id-authentication-0b28a127d66a
public class CustomAuthStateProvider : AuthenticationStateProvider
{
	private const string PrincipalKey = "principal_identity";
	private readonly HttpClient _httpClient;
	private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

	public CustomAuthStateProvider(HttpClient httpClient)
	{
		_httpClient = httpClient;
		_ = LoadAuthStateAsync();
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
		var user = await LoginWithExternalProviderAsync();
		_currentUser = user;

		// Persist the claims principal identity
		await SavePrincipalIdentityAsync(_currentUser);

		return new AuthenticationState(_currentUser);
	}

	private async Task<ClaimsPrincipal> LoginWithExternalProviderAsync()
	{
		var pcaOptions = new PublicClientApplicationOptions
		{
			ClientId = "3469f319-54f9-42d5-b2af-4d24c06994dc",
			TenantId = "common",
			RedirectUri = "http://localhost:5002",
			Instance = "https://uicrafters.b2clogin.com/tfp",
		};

		var pca = PublicClientApplicationBuilder.CreateWithApplicationOptions(pcaOptions)
			.WithB2CAuthority("https://uicrafters.b2clogin.com/tfp/uicrafters.onmicrosoft.com/b2c_1_login-register")
			.Build();

		var scopes = new[] { "openid", "offline_access" };
		var result = await pca.AcquireTokenInteractive(scopes).ExecuteAsync();

		var claims = new List<Claim> { new Claim("AccessToken", result.IdToken) };
		claims.AddRange(result.ClaimsPrincipal.Claims);

		var identity = new ClaimsIdentity(claims, "Custom", "name", ClaimTypes.Role);
		var principal = new ClaimsPrincipal(identity);

		var accessToken = result.IdToken;

		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

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
		SecureStorage.Default.Remove(PrincipalKey);
		NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
	}

	private async Task LoadAuthStateAsync()
	{
		var principalJson = await SecureStorage.Default.GetAsync(PrincipalKey);
		if (!string.IsNullOrEmpty(principalJson))
		{
			var claimsPrincipal = DeserializePrincipal(principalJson);
			if (claimsPrincipal != null)
			{
				_currentUser = claimsPrincipal;
				NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
			}
		}
	}

	private async Task SavePrincipalIdentityAsync(ClaimsPrincipal principal)
	{
		var principalJson = SerializePrincipal(principal);
		await SecureStorage.Default.SetAsync(PrincipalKey, principalJson);
	}

	private string SerializePrincipal(ClaimsPrincipal principal)
	{
		var claims = principal.Claims.Select(c => new { c.Type, c.Value }).ToList();
		return JsonSerializer.Serialize(claims);
	}

	private ClaimsPrincipal DeserializePrincipal(string json)
	{
		var claimsData = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(json);
		if (claimsData == null)
		{
			return new ClaimsPrincipal(new ClaimsIdentity());
		}

		var claims = claimsData.Select(c => new Claim(c["Type"], c["Value"])).ToList();
		var identity = new ClaimsIdentity(claims, "Custom", "name", ClaimTypes.Role);
		return new ClaimsPrincipal(identity);
	}
}
