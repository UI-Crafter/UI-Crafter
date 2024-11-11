namespace UICrafter.Core.Utility;

using System.Text.Json.Nodes;
using static MudBlazor.CategoryTypes;

//public class JsonProcessor
//{
//	/// <summary>
//	/// Processes any JSON string and extracts all fields and their values.
//	/// </summary>
//	/// <param name="json">The JSON string to process.</param>
//	/// <returns>A dictionary containing field names and their corresponding values.</returns>
//	public static Dictionary<string, object> ProcessJson(string json)
//	{
//		var result = new Dictionary<string, object>();
//		var node = JsonNode.Parse(json)!;
//		ProcessNode(node, result);
//		return result;
//	}

//	private static void ProcessNode(JsonNode node, Dictionary<string, object> result, string parentName = "")
//	{
//		if (node is JsonObject obj)
//		{
//			foreach (var property in obj)
//			{
//				ProcessNode(property.Value!, result, property.Key);
//			}
//		}
//		else if (node is JsonArray array)
//		{
//			var items = new List<object>();
//			foreach (var item in array)
//			{
//				if (item is JsonObject or JsonArray)
//				{
//					var childResult = new Dictionary<string, object>();
//					ProcessNode(item, childResult);
//					items.Add(childResult);
//				}
//				else
//				{
//					items.Add(item.GetValue<object>());
//				}
//			}
//			result[parentName] = items;
//		}
//		else if (node is JsonValue value)
//		{
//			result[parentName] = value.GetValue<object>();
//		}
//	}
//}

//chat try
//public class JsonProcessor
//{
//	public static Dictionary<string, object> ProcessJson(string json)
//	{
//		var result = new Dictionary<string, object>();
//		var node = JsonNode.Parse(json)!;
//		ProcessNode(node, result);
//		return result;
//	}

//	private static void ProcessNode(JsonNode node, Dictionary<string, object> result, string parentName = "")
//	{
//		if (node is JsonObject obj)
//		{
//			foreach (var property in obj)
//			{
//				string fieldName = property.Key;
//				if (!string.IsNullOrEmpty(parentName) && parentName != "jokes")
//				{
//					fieldName = parentName; // Only add the root names like "jokes" to keep top-level structure
//				}
//				ProcessNode(property.Value!, result, fieldName);
//			}
//		}
//		else if (node is JsonArray array)
//		{
//			foreach (var item in array)
//			{
//				ProcessNode(item, result, parentName);
//			}
//			result[parentName] = node;
//		}
//		else if (node is JsonValue value)
//		{
//			AddToDictionary(result, parentName, value.GetValue<object>());
//		}
//	}

//	private static void AddToDictionary(Dictionary<string, object> result, string key, object value)
//	{
//		// Check if the key already exists
//		if (result.ContainsKey(key))
//		{
//			// Concatenate the new value with the existing one
//			result[key] = result[key] + "\n" + value.ToString();
//		}
//		else
//		{
//			result[key] = value.ToString();
//		}
//	}
//}

//advanved json chat
public class JsonProcessor
{
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
				string fieldName = property.Key;
				//if (!string.IsNullOrEmpty(parentName) && parentName != "jokes")
				//{
				//	fieldName = parentName; // Only add root names like "jokes" to keep top-level structure
				//}
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
			//AddToDictionary(result, parentName, node);
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
			// Convert dictionary items (e.g., flags) to "key: value" format and join them into a single string
			string formattedFlags = string.Join("\n", dictionary.Select(kv => $"{kv.Key}: {kv.Value}"));
			value = formattedFlags;
		}

		// Check if the key already exists
		if (result.ContainsKey(key))
		{
			// Concatenate the new value with the existing one
			result[key] = result[key] + "\n\n" + value.ToString();
		}
		else
		{
			result[key] = value.ToString();
		}
	}
}

