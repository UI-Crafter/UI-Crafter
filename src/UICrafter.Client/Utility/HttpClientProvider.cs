namespace UICrafter.Client.Utility;

using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using UICrafter.Core.Utility;

public class HttpClientProvider : IHttpClientProvider
{
	private readonly HttpClient _httpClient;

	public HttpClientProvider(HttpClient httpClient) => _httpClient = httpClient;

	public Task<HttpClient> GetAuthenticatedHttpClient() => Task.FromResult(_httpClient);

	public HttpClient GetDefaultHttpClient() => _httpClient;
}
