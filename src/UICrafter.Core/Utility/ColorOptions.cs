namespace UICrafter.Core.Utility;
using UICrafter.Core.Models;
using MudBlazor;


public class ColorOptions
{
	public static Color GetColor(BaseModel Model)
	{
		return Model.Color?.ToLower() switch
		{
			"primary" => Color.Primary,
			"secondary" => Color.Secondary,
			"success" => Color.Success,
			"error" => Color.Error,
			"warning" => Color.Warning,
			"info" => Color.Info,
			_ => Color.Default
		};
	}
}
