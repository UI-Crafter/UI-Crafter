using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using UICrafter;
using UICrafter.API;
using UICrafter.Components;
using UICrafter.Core.Repository;
using UICrafter.Core.Utility;
using UICrafter.EntityConfigurations;
using UICrafter.Repository;
using UICrafter.Services;
using UICrafter.Utility;
using Microsoft.OpenApi.Models;
using UICrafter.Core.DependencyInjection;
using UICrafter.Options;
using UICrafter.Proxy;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSeriloggerSetup(builder.Configuration);

// Add MudBlazor services
builder.Services.AddUICrafterMudServices();

// Http setup
builder.Services.AddHttpClient();
builder.Services.AddScoped<IHttpClientProvider, HttpClientProvider>();

// gRPC
if (builder.Environment.IsDevelopment())
{
	builder.Services.AddGrpc().AddJsonTranscoding();
	builder.Services.AddGrpcSwagger().AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "gRPC transcoding", Version = "v1" }));
}
else
{
	builder.Services.AddGrpc();
}

// Swagger setup
if (builder.Environment.IsDevelopment())
{
	builder.Services.AddEndpointsApiExplorer();
}

// Repository
builder.Services.AddScoped<IAppViewRepository, AppViewRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// API Call handler
builder.Services.AddScoped<IAPICallHandler, DefaultAPICallHandler>();

// Database setup
builder.Services.AddDbContext<ApplicationDbContext>(options =>
		options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

// Auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddMicrosoftIdentityWebApi(builder.Configuration)
	.EnableTokenAcquisitionToCallDownstreamApi()
	.AddInMemoryTokenCaches();

builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
	var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtBearerAuthenticationOptions>()!;
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidAudiences = jwtSettings.ValidAudiences,
	};

	// Prevents redirect to login for API
	options.Events = new JwtBearerEvents
	{
		OnChallenge = context =>
		{
			// Suppress the redirect to login and instead return 401
			context.HandleResponse();
			context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			return Task.CompletedTask;
		},
		OnAuthenticationFailed = context =>
		{
			// Handle authentication failures, if needed, by customizing the response
			context.NoResult();
			context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			return Task.CompletedTask;
		}
	};
});

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
	.AddMicrosoftIdentityWebApp(builder.Configuration);
builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options => OpenIdConnectConfiguration.Configure(options, builder.Configuration));
builder.Services.AddScoped<AuthenticationStateProvider, PersistingAuthenticationStateProvider>();

builder.Services.AddAuthorizationBuilder()
	.SetDefaultPolicy(new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme)
	.RequireAuthenticatedUser()
	.Build());

builder.Services.AddCascadingAuthenticationState();

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents()
	.AddInteractiveWebAssemblyComponents();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
	app.UseSwagger();
	app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));
}
else
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseGrpcWeb();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode()
	.AddInteractiveWebAssemblyRenderMode()
	.AddAdditionalAssemblies(typeof(UICrafter.Client._Imports).Assembly);

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("auth/login", (string? returnUrl) => TypedResults.Challenge(new AuthenticationProperties { RedirectUri = returnUrl }))
			.AllowAnonymous();

app.MapGroup("user/").RequireAuthorization().MapUserAPI();

app.MapGrpcService<AppViewServicegRPC>().EnableGrpcWeb().RequireAuthorization();

app.MapGroup("proxy/forwarder").RequireAuthorization().MapUICrafterProxy();
app.Run();
