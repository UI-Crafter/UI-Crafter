namespace UICrafter.Core.Utility;

using System.Text.RegularExpressions;
using System.Text;

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
			return string.Join("\n\n", list.Select(ObjectAsString));
		}
		else
		{
			return valueObject?.ToString() ?? "null";
		}
	}
	public static bool TryGetNestedFieldValue(object current, string[] fieldPath, out object? result)
	{
		result = current;

		foreach (var field in fieldPath)
		{
			if (result is Dictionary<string, object> dict && dict.TryGetValue(field, out var tempResult))
			{
				result = tempResult;
			}
			else if (result is List<object> list && int.TryParse(field, out var index) && index < list.Count)
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
		char[] charsToRemove = {'\"'};
		StringBuilder cleanedString = new StringBuilder();

		var decodedInput = Regex.Replace(input, @"\\u([0-9A-Fa-f]{4})", match =>
		{
			return ((char)int.Parse(match.Groups[1].Value, System.Globalization.NumberStyles.HexNumber)).ToString();
		});

		foreach (char c in decodedInput)
		{
			if (!charsToRemove.Contains(c))
			{
				cleanedString.Append(c);
			}
		}

		return cleanedString.ToString();
	}
}
