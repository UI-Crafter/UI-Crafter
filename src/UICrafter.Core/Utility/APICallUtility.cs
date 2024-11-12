namespace UICrafter.Core.Utility;

using UICrafter.Core.UIComponents;

public static class APICallUtility
{
	public static string ReplaceLogicalNames(IEnumerable<UIComponent> UIComponents, string s)
	{
		foreach (var (key, value) in UIComponents.Where(x => x.ComponentCase == UIComponent.ComponentOneofCase.InputField).Select(x => (x.InputField.LogicalName, x.InputField.Value)))
		{
			s = s.Replace($"{{{key}}}", value);
		}

		return s;
	}
}
