namespace UICrafter;

using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using UICrafter.Models;
using Microsoft.Identity.Web;
using UICrafter.Extensions;
using UICrafter.Repository;

public static class OpenIdConnectConfiguration
{
	public static void Configure(OpenIdConnectOptions options)
	{
		options.Events.OnRemoteSignOut = async context =>
		{
			var request = context.Request;
			string redirectUri;

			if (request.Query.TryGetValue("post_logout_redirect_uri", out var stringValue))
			{
				redirectUri = stringValue.ToString();
			}
			else
			{
				var baseAddress = $"{request.Scheme}://{request.Host}{request.PathBase}";
				redirectUri = baseAddress;
			}

			await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			var logoutUri = $"https://uicrafters.b2clogin.com/uicrafters.onmicrosoft.com/b2c_1_login-register/oauth2/v2.0/logout?post_logout_redirect_uri={Uri.EscapeDataString(redirectUri)}";

			context.Response.Redirect(logoutUri);
			context.HandleResponse();

			await Task.CompletedTask;
		};

		options.Events.OnRedirectToIdentityProvider = async context => Console.WriteLine("OnRedirectToIdentityProvider");

		options.Events.OnRemoteFailure = context =>
		{
			var message = context.Failure?.Message;

			var errorBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(message));

			var redirectUrl = $"/auth/errorpage?errorDetails={errorBase64}";
			context.Response.Redirect(redirectUrl);
			context.HandleResponse();
			return Task.CompletedTask;
		};

		options.Events.OnTokenValidated = context =>
		{
			var user = context.Principal;

			ArgumentNullException.ThrowIfNull(user);

			var userEntity = new UserEntity
			{
				Id = user.GetAzureId(),
				Name = user.GetName(),
				Email = user.GetEmail(),
			};

			var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

			userRepository.UpsertUserEntity(userEntity);

			return Task.CompletedTask;
		};

		options.Prompt = "login";
	}
}
