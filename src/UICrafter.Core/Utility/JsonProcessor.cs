namespace UICrafter.Core.Utility;

using System.Text.Json.Nodes;

public static class JsonProcessor
{
	public static Dictionary<string, object> ProcessJson(string json)
	{
		var result = new Dictionary<string, object>();
		var node = JsonNode.Parse(json)!;
		AddToDictionary(result, "response", node);
		ProcessNode(node, result);
		return result;
	}

	private static void ProcessNode(JsonNode node, Dictionary<string, object> result, string parentName = "")
	{
		if (node is JsonObject obj)
		{
			foreach (var property in obj)
			{
				string fieldName = property.Key;
				ProcessNode(property.Value!, result, fieldName);
			}
			AddToDictionary(result, parentName, node);
		}
		else if (node is JsonArray array)
		{
			foreach (var item in array)
			{
				ProcessNode(item, result, parentName);
			}
		}
		else if (node is JsonValue value)
		{
			AddToDictionary(result, parentName, value.GetValue<object>());
		}
	}

	private static void AddToDictionary(Dictionary<string, object> result, string key, object value)
	{
		if (value is Dictionary<string, object> dictionary)
		{
			string formattedFlags = string.Join("\n", dictionary.Select(kv => $"{kv.Key}: {kv.Value}"));
			value = formattedFlags;
		}

		if (result.ContainsKey(key))
		{
			result[key] = result[key] + "\n\n" + value.ToString();
		}
		else
		{
			result[key] = value.ToString();
		}
	}
}

