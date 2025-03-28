using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using UICrafter.Client;
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
builder.Services.AddUICrafterMudServices();

// gRPC
builder.Services.AddGrpcClient<AppViewService.AppViewServiceClient>(options => options.Address = new Uri(builder.HostEnvironment.BaseAddress))
	.ConfigurePrimaryHttpMessageHandler(() => new GrpcWebHandler(new HttpClientHandler()));

// Repository
builder.Services.AddScoped<IAppViewRepository, DefaultAppViewRepository>();

// API Call handler
builder.Services.AddScoped<IAPICallHandler, ProxyAPICallHandler>();

await builder.Build().RunAsync();
