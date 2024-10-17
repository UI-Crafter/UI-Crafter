namespace UICrafter.Mobile.Utility;

using System.Net.Http;
using UICrafter.Core.Utility;

public class HttpClientProvider : IHttpClientProvider
{
	private readonly HttpClient _httpClient;

	public HttpClientProvider(HttpClient httpClient) => _httpClient = httpClient;

	public HttpClient GetDefaultHttpClient() => _httpClient;
}
