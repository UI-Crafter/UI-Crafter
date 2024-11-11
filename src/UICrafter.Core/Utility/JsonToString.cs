namespace UICrafter.Core.Utility;

using System.Text.RegularExpressions;
using System.Text;

public class JsonToString
{
	public static string? ObjectAsString(object valueObject)
	{
		return valueObject is Dictionary<string, object> dict
			? dict.Select(kv => $"\"{kv.Key}\": {ObjectAsString(kv.Value)}").ToString()
			: valueObject?.ToString() ?? "null";
	}
	public static bool TryGetNestedFieldValue(object current, string field, out object? result)
	{
		result = current;

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

		return true;
	}


	public static string CleanUpString(string input)
	{
		char[] charsToRemove = { '\"' };
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
