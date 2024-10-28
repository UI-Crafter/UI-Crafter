namespace UICrafter.Utility;

using System.Net.Http;
using System.Threading.Tasks;
using UICrafter.Core.Utility;

public class HttpClientProvider : IHttpClientProvider
{
	private readonly IHttpClientFactory _httpClientFactory;

	public HttpClientProvider(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

	public Task<HttpClient> GetAuthenticatedHttpClient() => Task.FromResult(GetDefaultHttpClient());

	public HttpClient GetDefaultHttpClient() => _httpClientFactory.CreateClient();
}
