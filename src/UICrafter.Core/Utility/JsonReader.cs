namespace UICrafter.Core.Utility;

using System.Text.Json.Nodes;

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
		var node = JsonNode.Parse(json)!;
		ProcessNode(node, result);
		return result;
	}

	private static void ProcessNode(JsonNode node, Dictionary<string, object> result, string parentName = "")
	{
		if (node is JsonObject obj)
		{
			foreach (var property in obj)
			{
				ProcessNode(property.Value!, result, property.Key);
			}
		}
		else if (node is JsonArray array)
		{
			var items = new List<object>();
			foreach (var item in array)
			{
				if (item is JsonObject or JsonArray)
				{
					var childResult = new Dictionary<string, object>();
					ProcessNode(item, childResult);
					items.Add(childResult);
				}
				else
				{
					items.Add(item.GetValue<object>());
				}
			}
			result[parentName] = items;
		}
		else if (node is JsonValue value)
		{
			result[parentName] = value.GetValue<object>();
		}
	}
}

