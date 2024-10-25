namespace UICrafter.Mobile.Utility;

using System.Net.Http;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using UICrafter.Core.Utility;

public class HttpClientProvider : IHttpClientProvider
{
	private readonly HttpClient _httpClient;
	private readonly AuthenticationStateProvider _authenticationStateProvider;

	public HttpClientProvider(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider)
	{
		_httpClient = httpClient;
		_authenticationStateProvider = authenticationStateProvider;
	}

	public HttpClient GetDefaultHttpClient() => _httpClient;

	public async Task<HttpClient> GetAuthenticatedHttpClient()
	{
		var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
		var user = authState.User;

		// Check if the user is authenticated
		if (user?.Identity?.IsAuthenticated != true)
		{
			throw new InvalidOperationException("The user is not authenticated.");
		}

		// Retrieve the access token from claims
		var accessToken = user.Claims.FirstOrDefault(c => c.Type == "AccessToken")?.Value;

		if (string.IsNullOrEmpty(accessToken))
		{
			throw new InvalidOperationException("Access token is missing.");
		}

		// Add the bearer token to the Authorization header
		_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
		return _httpClient;
	}
}
