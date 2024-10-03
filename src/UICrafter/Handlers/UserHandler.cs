namespace UICrafter.Handlers;

using System.Collections.Generic;
using UICrafter.Core.Handlers;

public class UserHandler : IUserHandler
{
	public IEnumerable<dynamic> GetUsers() => throw new NotImplementedException(); // EF Core
}
