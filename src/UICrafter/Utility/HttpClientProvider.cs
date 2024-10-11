namespace UICrafter.Utility;

using System.Net.Http;
using UICrafter.Core.Utility;

public class HttpClientProvider : IHttpClientProvider
{
	private readonly IHttpClientFactory _httpClientFactory;

	public HttpClientProvider(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

	public HttpClient GetDefaultHttpClient() => _httpClientFactory.CreateClient();
}
