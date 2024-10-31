namespace UICrafter.Mobile;

using Microsoft.JSInterop;
using Serilog.Core;
using Serilog.Events;

public class BlazorHybridConsoleSink : ILogEventSink
{
	private readonly IJSRuntime _jsRuntime;

	public BlazorHybridConsoleSink(IJSRuntime jsRuntime) => _jsRuntime = jsRuntime;

	public void Emit(LogEvent logEvent)
	{
		var logMessage = logEvent.RenderMessage();
		_jsRuntime.InvokeVoidAsync("console.log", logMessage);
	}
}
