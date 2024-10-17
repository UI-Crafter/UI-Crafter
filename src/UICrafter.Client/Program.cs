using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using UICrafter.Client.Utility;
using UICrafter.Core.AppView;
using UICrafter.Core.Utility;
using UICrafter.Core.DependencyInjection;
using UICrafter.Core.Repository;
using UICrafter.Client.Repository;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IHttpClientProvider, HttpClientProvider>();
builder.Services.AddMudServices();

// gRPC
builder.Services.AddGrpcClient<AppViewService.AppViewServiceClient>();

// Repository
builder.Services.AddScoped<IAppViewRepository, AppViewRepository>();

await builder.Build().RunAsync();
