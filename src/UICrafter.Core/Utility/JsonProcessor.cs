namespace UICrafter.Core.Utility;

using System.Collections.Generic;
using System.Text.Json.Nodes;

public static class JsonProcessor
{
	public static Dictionary<string, string> ProcessJson(string json)
	{
		var result = new Dictionary<string, string>();
		var node = JsonNode.Parse(json)!;
		AddToDictionary(result, "response", node);
		ProcessNode(node, result);
		return result;
	}

	private static void ProcessNode(JsonNode node, Dictionary<string, string> result, string parentName = "")
	{
		if (node is JsonObject obj)
		{
			foreach (var property in obj)
			{
				ProcessNode(property.Value!, result, property.Key);
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

	private static void AddToDictionary(Dictionary<string, string> result, string key, object value)
	{
		if (value is Dictionary<string, string> dictionary)
		{
			value = dictionary.Select(kv => $"{kv.Key}: {kv.Value}").ToString()!;
		}

		if (!result.ContainsKey(key))
		{
			result[key] = value.ToString()!;
		}
		else
		{
			result[key] = result[key] + "\n \n" + value.ToString();
		}
	}
}

