namespace UICrafter.Core.UIComponents;


public abstract class UIComponent
{
	public Guid Guid { get; set; } = Guid.NewGuid();
	public string DropZoneID { get; set; } = "Drop Zone 1";
}

public class UIButton : UIComponent
{
	public string Label { get; set; } = string.Empty;
	public string URL { get; set; } = string.Empty;
}

public class UIInputField : UIComponent
{
	public string Label { get; set; } = string.Empty;
	public string Value { get; set; } = string.Empty;
}

public class UITextBox : UIComponent
{
	public string Label { get; set; } = string.Empty;
	public int NumberOfLines { get; set; } = 2;
	public string Content { get; set; } = string.Empty;
	public Guid SourceRef { get; set; } = Guid.Empty;
	public string JsonField { get; set; } = string.Empty;
}

