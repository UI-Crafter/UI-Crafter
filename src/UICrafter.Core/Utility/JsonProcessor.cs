namespace UICrafter.Core.Utility;

using System.Collections.Generic;
using System.Text.Json.Nodes;

public static class JsonProcessor
{
	public static Dictionary<string, string> ProcessJson(string json)
	{
		var result = new Dictionary<string, string>
		{
			["response"] = json
		};
		var node = JsonNode.Parse(json)!;
		ProcessNode(node, result);
		return result;
	}

	private static void ProcessNode(JsonNode node, Dictionary<string, string> result, string parentName = "")
	{
		if (node is JsonObject obj)
		{
			foreach (var property in obj)
			{
				if (property.Value is null)
				{
					AddToDictionary(result, property.Key, "null");
				}
				ProcessNode(property.Value!, result, property.Key);
			}
			AddToDictionary(result, parentName, node);
		}
		else if (node is JsonArray array)
		{
			foreach (var item in array)
			{
				if (item is not null)
				{
					ProcessNode(item, result, parentName);
				}
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

		if (!result.TryAdd(key, value.ToString() ?? "null"))
		{
			result[key] = result[key] + "\n \n" + value.ToString();
		}
	}
}

