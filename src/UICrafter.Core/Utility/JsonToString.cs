namespace UICrafter.Core.Utility;

using System.Text.RegularExpressions;
using System.Text;

public class JsonToString
{
	public static string CleanUpString(string input)
	{
		char[] charsToRemove = { '\"', '[', ']', '(', ')', '{', '}' };
		var cleanedString = new StringBuilder();

		var decodedInput = Regex.Replace(input, @"\\u([0-9A-Fa-f]{4})", match => ((char)int.Parse(match.Groups[1].Value, System.Globalization.NumberStyles.HexNumber)).ToString());

		foreach (var c in decodedInput)
		{
			if (!charsToRemove.Contains(c))
			{
				cleanedString.Append(c);
			}
		}

		return cleanedString.ToString();
	}
}
