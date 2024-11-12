namespace UICrafter.Core.Utility;

using System.Text;
using UICrafter.Core.UIComponents;

public static class APICallUtility
{
	public static string ReplaceLogicalNames(IEnumerable<UIComponent> UIComponents, string s)
	{
		StringBuilder sb = new(s);
		foreach (var component in UIComponents.Where(x => x.ComponentCase == UIComponent.ComponentOneofCase.InputField))
		{
			sb.Replace($"{{{component.InputField.LogicalName}}}", component.InputField.Value);
		}

		return sb.ToString();
	}
}
