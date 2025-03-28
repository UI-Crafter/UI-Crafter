namespace UICrafter.Core.Utility;

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using UICrafter.Core.UIComponents;

public class DefaultAPICallHandler : IAPICallHandler
{
	private readonly IHttpClientProvider _httpClientProvider;

	public DefaultAPICallHandler(IHttpClientProvider httpClientProvider) => _httpClientProvider = httpClientProvider;

	public async Task<HttpResponseMessage> ExecuteHttpRequest(IEnumerable<UIComponent> UIComponents, CallMethod httpMethod, string url, RepeatedField<HttpHeader> headers, string body)
	{
		var httpClient = _httpClientProvider.GetDefaultHttpClient();

		var method = httpMethod switch
		{
			CallMethod.Get => HttpMethod.Get,
			CallMethod.Post => HttpMethod.Post,
			CallMethod.Put => HttpMethod.Put,
			CallMethod.Delete => HttpMethod.Delete,
			_ => throw new InvalidOperationException($"Unsupported HTTP method: {httpMethod}")
		};

		using var request = new HttpRequestMessage(method, url);

		if (headers.Count > 0)
		{
			foreach (var header in headers)
			{
				request.Headers.Add(header.Key, header.Value);
			}
		}

		if (!string.IsNullOrWhiteSpace(body))
		{
			body = APICallUtility.ReplaceLogicalNames(UIComponents, body);
			request.Content = new StringContent(body);
		}

		return await httpClient.SendAsync(request);
	}
}
