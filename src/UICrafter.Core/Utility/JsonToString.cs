namespace UICrafter.Core.Utility;

using System.Text.RegularExpressions;
using System.Text;
using System.Text.Json;

public class JsonToString
{
	public static string ObjectAsString(object valueObject)
	{
		if (valueObject is Dictionary<string, object> dict)
		{
			return string.Join("\n\n", dict.Select(kv => $"\"{kv.Key}\": {ObjectAsString(kv.Value)}"));
		}
		else if (valueObject is List<object> list)
		{
			return string.Join("\n\n", list.Select(item => ObjectAsString(item)));
		}
		else
		{
			return valueObject?.ToString() ?? "null";
		}
	}
	public static bool TryGetNestedFieldValue(object current, string[] fieldPath, out object result)
	{
		result = current;

		foreach (var field in fieldPath)
		{
			if (result is Dictionary<string, object> dict && dict.TryGetValue(field, out var tempResult))
			{
				result = tempResult;
			}
			else if (result is List<object> list && int.TryParse(field, out int index) && index < list.Count)
			{
				result = list[index];
			}
			else
			{
				result = null;
				return false;
			}
		}

		return true;
	}


	public static string CleanUpString(string input)
	{
		// Characters to be removed directly
		char[] charsToRemove = { '\"' };
		StringBuilder cleanedString = new StringBuilder();

		// Decode Unicode escape sequences (e.g., \u0022) into actual characters
		var decodedInput = Regex.Replace(input, @"\\u([0-9A-Fa-f]{4})", match =>
		{
			// Convert the matched Unicode code to a character
			return ((char)int.Parse(match.Groups[1].Value, System.Globalization.NumberStyles.HexNumber)).ToString();
		});

		// Process the decoded string to remove unwanted characters
		foreach (char c in decodedInput)
		{
			// Only append characters that are not in charsToRemove
			if (!charsToRemove.Contains(c))
			{
				cleanedString.Append(c);
			}
		}

		return cleanedString.ToString();
	}

	// public string CleanUpString(string input)
	// {
	//     // Characters to be removed
	//     //char[] charsToRemove = { '\"', '\'', '{', '}', '[', ']', '(', ')' };
	//     char[] charsToRemove = { '\"'};
	//     System.Text.StringBuilder cleanedString = new StringBuilder();

	//     foreach (char c in input)
	//     {
	//         // Only append characters that are not in charsToRemove
	//         if (!charsToRemove.Contains(c))
	//         {
	//             cleanedString.Append(c);
	//         }
	//     }

	//     return cleanedString.ToString();
	// }

}
