namespace UICrafter.Mobile.Utility;

using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Authorization;
using UICrafter.Core.Utility;

public partial class HttpClientProvider : IHttpClientProvider, IDisposable
{
	private readonly HttpClient _defaulthttpClient = new();
	private readonly HttpClient _authenticatedhttpClient;
	private readonly AuthenticationStateProvider _authenticationStateProvider;

	public HttpClientProvider(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider)
	{
		_authenticatedhttpClient = httpClient;
		_authenticationStateProvider = authenticationStateProvider;
	}

	public HttpClient GetDefaultHttpClient() => _defaulthttpClient;

	public async Task<HttpClient> GetAuthenticatedHttpClient()
	{
		var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
		var user = authState.User;

		if (user?.Identity?.IsAuthenticated != true)
		{
			throw new InvalidOperationException("The user is not authenticated.");
		}

		var accessToken = user.Claims.FirstOrDefault(c => c.Type == "AccessToken")?.Value;

		if (string.IsNullOrWhiteSpace(accessToken))
		{
			throw new InvalidOperationException("Access token is missing.");
		}

		_authenticatedhttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
		return _authenticatedhttpClient;
	}

	public void Dispose()
	{
		_defaulthttpClient.Dispose();
		GC.SuppressFinalize(this);
	}
}
