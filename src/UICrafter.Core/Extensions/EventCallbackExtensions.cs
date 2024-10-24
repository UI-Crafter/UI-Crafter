namespace UICrafter.Core.Extensions;

using Microsoft.AspNetCore.Components;

public static class EventCallbackExtensions
{
	/// <summary>
	/// Invokes delegate if available associated with this binding and dispatches an event notification to the
	/// appropriate component.
	/// </summary>
	/// <param name="arg">The argument.</param>
	/// <returns>A <see cref="Task"/> which completes asynchronously once event processing has completed.</returns>
	public static Task TryInvokeAsync<T>(this EventCallback<T> callback, T? arg) => callback.HasDelegate ? callback.InvokeAsync(arg) : Task.CompletedTask;
}
