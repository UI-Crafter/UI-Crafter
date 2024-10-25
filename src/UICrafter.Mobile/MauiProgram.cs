namespace UICrafter.Mobile;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using UICrafter.Core.Repository;
using UICrafter.Core.Utility;
using UICrafter.Core.DependencyInjection;
using UICrafter.Mobile.DependencyInjection;
using UICrafter.Mobile.Options;
using UICrafter.Mobile.Repository;
using UICrafter.Mobile.Utility;
using UICrafter.Core.AppView;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using UICrafter.Mobile.Extensions;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts => fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"));

		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddMudServices();

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

		var section = config.GetRequiredSection("ApiSettings");


		// Add Options and configure ApiSettings
		builder.Services.AddOptions<ApiSettings>().Bind(config.GetRequiredSection("ApiSettings")).ValidateDataAnnotations().ValidateOnStart();

		builder.Services.AddConfiguredHttpClient();
		// Register HttpClient using the configuration from IConfiguration
		builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(section.GetValue<string>("BaseUrl")!) });

		builder.Services.AddTransient<IHttpClientProvider, HttpClientProvider>();

		// Auth
		builder.Services.AddAuthorizationCore();
		builder.Services.TryAddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
		builder.Services.AddCascadingAuthenticationState();

		// gRPC
		builder.Services.AddGrpcClient<AppViewService.AppViewServiceClient>();

		// Repository
		builder.Services.AddScoped<IAppViewRepository, AppViewRepository>();

		return builder.Build();
	}
}
