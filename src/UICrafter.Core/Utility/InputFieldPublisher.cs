namespace UICrafter.Core.Utility;


public class InputFieldPublisher
{
	private readonly Dictionary<string, List<Action<string, string>>> _subscribers = [];

	public void Subscribe(string logicalName, Action<string, string> callback)
	{
		if (_subscribers.TryGetValue(logicalName, out var value))
		{
			value.Add(callback);
		}
		else
		{
			value = [callback];
			_subscribers[logicalName] = value;
		}
	}

	public void Unsubscribe(string logicalName, Action<string, string> callback)
	{
		if (_subscribers.TryGetValue(logicalName, out var value))
		{
			value.Remove(callback);

			if (value.Count == 0)
			{
				_subscribers.Remove(logicalName);
			}
		}
	}

	public void NotifySubscribers(string logicalName, string urlChange)
	{
		if (_subscribers.TryGetValue(logicalName, out var value))
		{
			foreach (var callback in value)
			{
				callback?.Invoke(logicalName, urlChange);
			}
		}
	}
}
