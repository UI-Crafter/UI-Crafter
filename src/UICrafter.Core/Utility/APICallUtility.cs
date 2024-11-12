namespace UICrafter.Core.Utility;

using UICrafter.Core.UIComponents;

public static class APICallUtility
{
	public static string ReplaceLogicalNames(IEnumerable<UIComponent> UIComponents, string s)
	{
		foreach (var component in UIComponents.Where(x => x.ComponentCase == UIComponent.ComponentOneofCase.InputField))
		{
			s = s.Replace($"{{{component.InputField.LogicalName}}}", component.InputField.Value);
		}

		return s;
	}
}
