namespace UICrafter.Core.Utility;
public interface IHttpClientProvider
{
	HttpClient GetDefaultHttpClient();
	Task<HttpClient> GetAuthenticatedHttpClient();
}
