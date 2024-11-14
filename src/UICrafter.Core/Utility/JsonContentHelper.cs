namespace UICrafter.Core.Utility;

using System.Text.Json;
using DevLab.JmesPath;
using Serilog;
using UICrafter.Core.UIComponents;

public static class JsonContentHelper
{
	public static string GetUpdatedContent(Dictionary<string, string> jsonResponse, UITextbox textbox)
	{
		if (string.IsNullOrWhiteSpace(textbox.JsonField))
		{
			return CleanUpString(jsonResponse["response"], textbox.CleanupTextBox);
		}
		else if (textbox.IsJsonQuery)
		{
			return UpdateContentJsonQuery(jsonResponse, textbox);
		}
		else
		{
			return UpdateContentJsonField(jsonResponse, textbox);
		}
	}

	private static string UpdateContentJsonQuery(Dictionary<string, string> jsonResponse, UITextbox textbox)
	{
		string? result = null;
		var _jmesPath = new JmesPath();
		try
		{
			result = _jmesPath.Transform(jsonResponse["response"], textbox.JsonField);
			var json = JsonSerializer.Deserialize<dynamic>(result);
			result = JsonSerializer.Serialize(json, new JsonSerializerOptions { WriteIndented = true });
		}
		catch (Exception ex)
		{
			Log.Warning($"API did not return valid JSON: {ex.Message}");
		}

		return result is not null
			? CleanUpString(result, textbox.CleanupTextBox)
			: "JsonQuery transformation failed.";
	}

	private static string UpdateContentJsonField(Dictionary<string, string> jsonResponse, UITextbox textbox)
	{
		return jsonResponse.TryGetValue(textbox.JsonField, out var field)
			? CleanUpString(field, textbox.CleanupTextBox)
			: $"Field '{textbox.JsonField}' not found in JSON response.";
	}

	private static string CleanUpString(string result, bool cleanupEnabled)
	{
		return cleanupEnabled ? JsonToString.CleanUpString(result) : result;
	}
}

