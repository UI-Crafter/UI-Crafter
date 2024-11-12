namespace UICrafter.Core.Utility;

using Google.Protobuf.Collections;
using UICrafter.Core.UIComponents;

public interface IAPICallHandler
{
	Task<HttpResponseMessage> ExecuteHttpRequest(IEnumerable<UIComponent> UIComponents, CallMethod httpMethod, string url, RepeatedField<HttpHeader> headers, string body);
	string ReplaceLogicalNames(IEnumerable<UIComponent> UIComponents, string s);
}
