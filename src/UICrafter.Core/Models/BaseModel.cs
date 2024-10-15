namespace UICrafter.Core.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum UItype
{
	Button,
	TextBox,
	Label
}

public class BaseModel
{
	public int? Id { get; set; }
	public string? Name { get; set; }
	public string? Description { get; set; }
	public UItype Type { get; set; }

	public string? Color { get; set; }

}

public class PocButtonModel : BaseModel
{
	public string? URL { get; set; }
	public string? JSONResponse { get; set; }
}

public class PocLabelModel : BaseModel
{

}

public class PocTextBoxModel : BaseModel
{
	public string? buttonGUID { get; set; }

	public string? JsonField { get; set; }
}

public class PocAPIParamInputFieldModel : BaseModel
{
	public int? ParamIndex { get; set; }
}
