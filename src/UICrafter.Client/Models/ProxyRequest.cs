namespace UICrafter.Client.Models;

public class ProxyRequest
{
	public string Url { get; set; } = string.Empty;
	public string Method { get; set; } = string.Empty;
	public Dictionary<string, string> Headers { get; set; } = [];
	public string? Body { get; set; } = string.Empty;
}
