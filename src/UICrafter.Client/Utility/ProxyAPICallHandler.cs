namespace UICrafter.Client.Utility;

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using UICrafter.Core.UIComponents;
using UICrafter.Core.Utility;

public class ProxyAPICallHandler : IAPICallHandler
{
	private readonly IHttpClientProvider _httpClientProvider;

	public ProxyAPICallHandler(IHttpClientProvider httpClientProvider) => _httpClientProvider = httpClientProvider;

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
			body = ReplaceLogicalNames(UIComponents, body);
			request.Content = new StringContent(body);
		}

		return await httpClient.SendAsync(request);
	}

	public string ReplaceLogicalNames(IEnumerable<UIComponent> UIComponents, string s)
	{
		foreach (var (key, value) in UIComponents.Where(x => x.ComponentCase == UIComponent.ComponentOneofCase.InputField).Select(x => (x.InputField.LogicalName, x.InputField.Value)))
		{
			s = s.Replace($"{{{key}}}", value);
		}

		return s;
	}
}
