namespace UICrafter.API;

using Microsoft.AspNetCore.Mvc;
using UICrafter.Extensions;
using UICrafter.Models;
using UICrafter.Repository;

public static class UserAPI
{
	public static RouteGroupBuilder MapUserAPI(this RouteGroupBuilder group)
	{
		group.RequireAuthorization();
		group.MapPut("authenticated", async (HttpContext context, [FromServices] IUserRepository userRepo) =>
		{
			var user = context.User;
			await userRepo.UpsertUserEntity(new UserEntity
			{
				Id = user.GetAzureId(),
				Email = user.GetEmail(),
				Name = user.GetName(),
			});
		});

		return group;
	}
}
