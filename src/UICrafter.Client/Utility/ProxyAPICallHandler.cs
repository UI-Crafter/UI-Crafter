namespace UICrafter.Client.Utility;

using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using UICrafter.Client.Models;
using UICrafter.Core.UIComponents;
using UICrafter.Core.Utility;

public class ProxyAPICallHandler : IAPICallHandler
{
	private readonly IHttpClientProvider _httpClientProvider;

	public ProxyAPICallHandler(IHttpClientProvider httpClientProvider) => _httpClientProvider = httpClientProvider;

	public async Task<HttpResponseMessage> ExecuteHttpRequest(IEnumerable<UIComponent> UIComponents, CallMethod httpMethod, string url, RepeatedField<HttpHeader> headers, string body)
	{
		var httpClient = _httpClientProvider.GetDefaultHttpClient();
		var proxyRequest = new ProxyRequest
		{
			Url = url,
			Method = httpMethod.ToString(),
			Headers = headers.ToDictionary(header => header.Key, header => header.Value),
			Body = string.IsNullOrWhiteSpace(body) ? null : APICallUtility.ReplaceLogicalNames(UIComponents, body)
		};

		var content = new StringContent(JsonSerializer.Serialize(proxyRequest), Encoding.UTF8, "application/json");

		var response = await httpClient.PostAsync("/proxy/forwarder", content);

		return response;
	}
}
