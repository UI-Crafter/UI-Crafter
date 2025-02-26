namespace UICrafter.Mobile;

using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using UICrafter.Core.AppView;
using UICrafter.Core.DependencyInjection;
using UICrafter.Core.Extensions;
using UICrafter.Core.Repository;
using UICrafter.Core.Utility;
using UICrafter.Mobile.DependencyInjection;
using UICrafter.Mobile.Extensions;
using UICrafter.Mobile.Options;
using UICrafter.Mobile.Utility;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts => fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"));

		builder.Logging.AddSeriloggerSetup(builder.Configuration);

		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddUICrafterMudServices();

		// Theme singleton
		builder.Services.AddSingleton<ThemeService>();

		var configBuilder = new ConfigurationBuilder()
			.AddEmbeddedResource("UICrafter.Mobile.appsettings.json");

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
		configBuilder.AddEmbeddedResource("UICrafter.Mobile.appsettings.Development.json");
#endif
		var config = configBuilder.Build();

		// Register IConfiguration as a service
		builder.Configuration.AddConfiguration(config);

		// Add Options and configure ApiSettings
		builder.Services.AddOptions<ApiSettings>().Bind(config.GetRequiredSection("ApiSettings")).ValidateDataAnnotations().ValidateOnStart();

		builder.Services.AddConfiguredHttpClient();

		builder.Services.AddScoped<IHttpClientProvider, HttpClientProvider>();

		// Auth
		builder.Services.AddAuthorizationCore();
		builder.Services.TryAddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
		builder.Services.AddCascadingAuthenticationState();

		// gRPC
		builder.Services.AddGrpcClient<AppViewService.AppViewServiceClient>((services, options) =>
			{
				var settings = services.GetRequiredService<IOptions<ApiSettings>>();

				options.Address = new Uri(settings.Value.BaseUrl);
			})
			.ConfigurePrimaryHttpMessageHandler(() =>
				new GrpcWebHandler(new HttpClientHandler()))
			.AddCallCredentials(async (context, metadata, services) =>
			{
				try
				{
					var authenticationStateProvider = services.GetRequiredService<AuthenticationStateProvider>();
					var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
					var user = authState.User;

					var token = user.GetAccessToken();

					ArgumentNullException.ThrowIfNull(token);

					metadata.Add("Authorization", $"Bearer {token}");
				}
				catch (Exception ex)
				{
					Log.Information("Failed adding gRPC auth header", ex);
				}
			});

		// Repository
		builder.Services.AddScoped<IAppViewRepository, DefaultAppViewRepository>();

		// API Call handler
		builder.Services.AddScoped<IAPICallHandler, DefaultAPICallHandler>();

		return builder.Build();
	}
}
