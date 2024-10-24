namespace UICrafter;

using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

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

		options.Events.OnRemoteFailure = async context =>
		{
			var message = context.Failure?.Message;

			var errorBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(message));

			var redirectUrl = $"/auth/errorpage?errorDetails={errorBase64}";
			context.Response.Redirect(redirectUrl);
			context.HandleResponse();
		};

		//options.Events.OnAuthenticationFailed = async context =>
		//{
		//	// Create an object with the error details
		//	var errorDetails = new
		//	{
		//		error = context.Exception.Message,
		//		error_description = context.ProtocolMessage.ErrorDescription,
		//		error_code = context.ProtocolMessage.Error,
		//		correlation_id = context.ProtocolMessage,
		//		trace_id = Guid.NewGuid().ToString(), // Example trace ID, change as needed
		//		timestamp = DateTime.UtcNow.ToString("o")
		//	};

		//	// Serialize the error object to JSON
		//	var errorJson = JsonSerializer.Serialize(errorDetails);

		//	// Convert the JSON string to a byte array and encode it as base64
		//	var errorBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(errorJson));

		//	// Redirect to the error page with the base64 encoded query parameter
		//	var redirectUrl = $"/authentication-error?errorDetails={errorBase64}";
		//	context.Response.Redirect(redirectUrl);
		//	context.HandleResponse();
		//};

		options.Events.OnTokenValidated = async context => Console.WriteLine("OnTokenValidated");

		options.Prompt = "login";
	}
}
