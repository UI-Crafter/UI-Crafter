namespace UICrafter.Proxy;

using Microsoft.AspNetCore.Mvc;
using UICrafter.Core.Utility;

public static class ProxyConfiguration
{
	public static IEndpointRouteBuilder MapUICrafterProxy(this IEndpointRouteBuilder builder)
	{
		builder.Map("", async (HttpContext context, [FromQuery] string uri, [FromServices] IHttpClientProvider httpProvider) =>
		{
			var httpClient = httpProvider.GetDefaultHttpClient();
			var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

			// Send the request and get the response
			var responseMessage = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

			// Set the status code from the response
			context.Response.StatusCode = (int)responseMessage.StatusCode;

			// Copy headers from the response
			foreach (var header in responseMessage.Headers)
			{
				context.Response.Headers[header.Key] = header.Value.ToArray();
			}
			foreach (var header in responseMessage.Content.Headers)
			{
				context.Response.Headers[header.Key] = header.Value.ToArray();
			}

			// Ensure no chunked transfer encoding, as it's handled automatically
			context.Response.Headers.Remove("transfer-encoding");

			// Stream the content to the response
			await responseMessage.Content.CopyToAsync(context.Response.Body);
		});

		return builder;
	}

}
