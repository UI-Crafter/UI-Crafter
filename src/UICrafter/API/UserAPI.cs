namespace UICrafter.API;

using Microsoft.AspNetCore.Mvc;
using UICrafter.Core.Extensions;
using UICrafter.Models;
using UICrafter.Repository;

public static class UserAPI
{
	public static IEndpointRouteBuilder MapUserAPI(this IEndpointRouteBuilder builder)
	{
		builder.MapPut("authenticated", async (HttpContext context, [FromServices] IUserRepository userRepo) =>
		{
			var user = context.User;
			await userRepo.UpsertUserEntity(new UserEntity
			{
				Id = user.GetAzureId(),
				Email = user.GetEmail(),
				Name = user.GetName(),
			});
		});

		return builder;
	}
}
