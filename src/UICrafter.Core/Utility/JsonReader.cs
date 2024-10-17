namespace UICrafter.Core.Utility;
using Newtonsoft.Json.Linq;

public class JsonProcessor
{
	/// <summary>
	/// Processes any JSON string and extracts all fields and their values.
	/// </summary>
	/// <param name="json">The JSON string to process.</param>
	/// <returns>A dictionary containing field names and their corresponding values.</returns>
	public static Dictionary<string, object> ProcessJson(string json)
	{
		var result = new Dictionary<string, object>();
		var token = JToken.Parse(json);
		ProcessToken(token, result);
		return result;
	}

	private static void ProcessToken(JToken token, Dictionary<string, object> result, string parentName = "")
	{
		if (token is JObject obj)
		{
			foreach (var property in obj.Properties())
			{
				ProcessToken(property.Value, result, property.Name);
			}
		}
		else if (token is JArray array)
		{
			var items = new List<object>();
			foreach (var item in array)
			{
				if (item.Type is JTokenType.Object or JTokenType.Array)
				{
					var childResult = new Dictionary<string, object>();
					ProcessToken(item, childResult);
					items.Add(childResult);
				}
				else
				{
					items.Add(((JValue)item).Value);
				}
			}
			result[parentName] = items;
		}
		else if (token is JValue value)
		{
			result[parentName] = value.Value;
		}
	}
}

