using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
using UICrafter.Client;
using UICrafter.Client.Repository;
using UICrafter.Client.Utility;
using UICrafter.Core.AppView;
using UICrafter.Core.DependencyInjection;
using UICrafter.Core.Repository;
using UICrafter.Core.Utility;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Logging.AddSeriloggerSetup(builder.Configuration);

// Auth setup
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IHttpClientProvider, HttpClientProvider>();
builder.Services.AddMudServices();

// gRPC
builder.Services.AddGrpcClient<AppViewService.AppViewServiceClient>(options => options.Address = new Uri(builder.HostEnvironment.BaseAddress));

// Repository
builder.Services.AddScoped<IAppViewRepository, AppViewRepository>();

//Snackbar settings
builder.Services.AddMudServices(config =>
{
	config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;

	config.SnackbarConfiguration.PreventDuplicates = true;
	config.SnackbarConfiguration.NewestOnTop = false;
	config.SnackbarConfiguration.ShowCloseIcon = true;
	config.SnackbarConfiguration.VisibleStateDuration = 1000;
	config.SnackbarConfiguration.HideTransitionDuration = 500;
	config.SnackbarConfiguration.ShowTransitionDuration = 500;
	config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

await builder.Build().RunAsync();
