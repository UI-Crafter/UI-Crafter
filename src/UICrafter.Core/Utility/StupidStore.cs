namespace UICrafter.Core.Utility;

public class StupidStore
{
	private readonly Dictionary<string, List<Action<Dictionary<string, object>>>> _subscribers = [];

	public void Subscribe(string buttonGUID, Action<Dictionary<string, object>> callback)
	{
		if (_subscribers.TryGetValue(buttonGUID, out var value))
		{
			value.Add(callback);
		}
		else
		{
			value = [callback];
			_subscribers[buttonGUID] = value;
		}
	}

	public void Unsubscribe(string buttonGUID, Action<Dictionary<string, object>> callback)
	{
		if (_subscribers.TryGetValue(buttonGUID, out var value))
		{
			value.Remove(callback);

			if (value.Count == 0)
			{
				_subscribers.Remove(buttonGUID); // Clean up if no subscribers are left
			}
		}
	}

	public void NotifySubscribers(string buttonGUID, string jsonResponse)
	{
		var processedData = JsonProcessor.ProcessJson(jsonResponse);
		if (_subscribers.TryGetValue(buttonGUID, out var value))
		{
			foreach (var callback in value)
			{
				callback?.Invoke(processedData);
			}
		}
	}
}
