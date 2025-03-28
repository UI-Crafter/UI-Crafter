namespace UICrafter;

using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using UICrafter.Core.Extensions;
using UICrafter.Models;
using UICrafter.Repository;

public static class OpenIdConnectConfiguration
{
	public static void Configure(OpenIdConnectOptions options, IConfiguration configuration)
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

			var logoutUri = $"{configuration["AzureAd:CustomLogoutUri"]}?post_logout_redirect_uri={Uri.EscapeDataString(redirectUri)}";

			context.Response.Redirect(logoutUri);
			context.HandleResponse();

			await Task.CompletedTask;
		};

		options.Events.OnRemoteFailure = context =>
		{
			var message = context.Failure?.Message;

			var errorBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(message));

			var redirectUrl = $"/auth/errorpage?errorDetails={errorBase64}";
			context.Response.Redirect(redirectUrl);
			context.HandleResponse();
			return Task.CompletedTask;
		};

		options.Events.OnTokenValidated = async context =>
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

			await userRepository.UpsertUserEntity(userEntity);
		};

		var redirectToIdpHandler = options.Events.OnRedirectToIdentityProvider;
		options.Events.OnRedirectToIdentityProvider = async context =>
		{
			await redirectToIdpHandler(context);

			if (context.ProtocolMessage.RedirectUri.StartsWith("http:", StringComparison.OrdinalIgnoreCase))
			{
				context.ProtocolMessage.RedirectUri = context.ProtocolMessage.RedirectUri.Replace("http:", "https:");
			}
		};

		var redirectToIdpForSignOutHandler = options.Events.OnRedirectToIdentityProviderForSignOut;
		options.Events.OnRedirectToIdentityProviderForSignOut = async context =>
		{
			await redirectToIdpForSignOutHandler(context);

			if (context.ProtocolMessage.RedirectUri.StartsWith("http:", StringComparison.OrdinalIgnoreCase))
			{
				context.ProtocolMessage.RedirectUri = context.ProtocolMessage.RedirectUri.Replace("http:", "https:");
			}
		};

		options.Prompt = "login";
	}
}
