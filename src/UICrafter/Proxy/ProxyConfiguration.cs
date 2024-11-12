namespace UICrafter.Proxy;

using Microsoft.AspNetCore.Mvc;
using UICrafter.Client.Models;
using UICrafter.Core.Utility;

public static class ProxyConfiguration
{
	public static IEndpointRouteBuilder MapUICrafterProxy(this IEndpointRouteBuilder builder)
	{
		builder.Map("", async (HttpContext context, [FromBody] ProxyRequest proxyRequest, [FromServices] IHttpClientProvider httpProvider) =>
		{
			var httpClient = httpProvider.GetDefaultHttpClient();

			var requestMessage = new HttpRequestMessage(new HttpMethod(proxyRequest.Method), proxyRequest.Url);

			foreach (var header in proxyRequest.Headers)
			{
				requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
			}

			if (!string.IsNullOrWhiteSpace(proxyRequest.Body))
			{
				requestMessage.Content = new StringContent(proxyRequest.Body);
			}

			var responseMessage = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

			context.Response.StatusCode = (int)responseMessage.StatusCode;
			foreach (var header in responseMessage.Headers.Concat(responseMessage.Content.Headers))
			{
				context.Response.Headers[header.Key] = header.Value.ToArray();
			}

			context.Response.Headers.Remove("transfer-encoding");
			await responseMessage.Content.CopyToAsync(context.Response.Body);
		});

		return builder;
	}
}
