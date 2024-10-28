using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Serilog;
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
builder.Services.AddGrpcClient<AppViewService.AppViewServiceClient>();

// Repository
builder.Services.AddScoped<IAppViewRepository, AppViewRepository>();

await builder.Build().RunAsync();
