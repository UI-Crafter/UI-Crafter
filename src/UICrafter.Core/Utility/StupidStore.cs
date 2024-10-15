namespace UICrafter.Core.Utility
{
	public class StupidStore
	{
		// Initialize JsonProcessor
		private readonly JsonProcessor _jsonProcessor = new();

		// Subscribers now expect callbacks that accept Dictionary<string, object>
		private readonly Dictionary<string, List<Action<Dictionary<string, object>>>> _subscribers = new();

		public void Subscribe(string buttonGUID, Action<Dictionary<string, object>> callback)
		{
			if (!_subscribers.ContainsKey(buttonGUID))
			{
				_subscribers[buttonGUID] = new List<Action<Dictionary<string, object>>>();
			}
			_subscribers[buttonGUID].Add(callback);
		}

		public void Unsubscribe(string buttonGUID, Action<Dictionary<string, object>> callback)
		{
			if (_subscribers.ContainsKey(buttonGUID))
			{
				_subscribers[buttonGUID].Remove(callback);
			}
		}

		public void NotifySubscribers(string buttonGUID, string jsonResponse)
		{
			var processedData = _jsonProcessor.ProcessJson(jsonResponse);
			if (_subscribers.ContainsKey(buttonGUID))
			{
				foreach (var callback in _subscribers[buttonGUID])
				{
					callback(processedData);
				}
			}
		}
	}
}
