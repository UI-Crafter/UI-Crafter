namespace UICrafter.Core.Utility;

using Microsoft.AspNetCore.Components.Authorization;

public interface IHttpClientProvider
{
	HttpClient GetDefaultHttpClient();
	Task<HttpClient> GetAuthenticatedHttpClient();
}
