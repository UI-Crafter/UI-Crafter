namespace UICrafter.Extensions;

public class ClaimNotFoundException : Exception
{
	public ClaimNotFoundException()
	{
	}

	public ClaimNotFoundException(string message)
		: base(message)
	{
	}

	public ClaimNotFoundException(string message, Exception inner)
		: base(message, inner)
	{
	}
}
