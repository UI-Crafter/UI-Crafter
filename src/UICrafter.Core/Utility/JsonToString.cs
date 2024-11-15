namespace UICrafter.Core.Utility;

using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;

public partial class JsonToString
{
	public static string CleanUpString(string input)
	{
		char[] charsToRemove = ['\"', '[', ']', '(', ')', '{', '}'];
		var cleanedString = new StringBuilder();

		var decodedInput = CleanStringRegex().Replace(input, match => ((char)int.Parse(match.Groups[1].Value, NumberStyles.HexNumber, new CultureInfo("en-US"))).ToString());

		foreach (var c in decodedInput)
		{
			if (!charsToRemove.Contains(c))
			{
				cleanedString.Append(c);
			}
		}

		return cleanedString.ToString();
	}

	[GeneratedRegex(@"\\u([0-9A-Fa-f]{4})")]
	private static partial Regex CleanStringRegex();
}
