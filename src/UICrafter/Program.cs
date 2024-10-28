using Google.Protobuf;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using UICrafter;
using UICrafter.API;
using UICrafter.Components;
using UICrafter.Core.Models;
using UICrafter.Core.Repository;
using UICrafter.Core.Utility;
using UICrafter.EntityConfigurations;
using UICrafter.Repository;
using UICrafter.Services;
using UICrafter.Utility;
using Microsoft.OpenApi.Models;
using UICrafter.Core.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSeriloggerSetup(builder.Configuration);

// Add MudBlazor services
builder.Services.AddMudServices();

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
builder.Services.AddEndpointsApiExplorer();

// Repository
builder.Services.AddScoped<IAppViewRepository, AppViewRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

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
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidAudiences =
		[
			"3469f319-54f9-42d5-b2af-4d24c06994dc", // Mobile App Registration Client ID
            "45698317-ca1e-4595-a069-3dad3bce31a6"  // Backend App Registration Client ID
        ],
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
	app.UseSwaggerUI();
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

app.MapGet("api/prototest", () =>
{
	// Create a new Person object
	var person = new Person { Email = "muppet@muppet.dk", Name = "Muppet" };

	// Serialize the person object to a binary byte array
	var serializedPerson = person.ToByteArray();

	// Convert the byte array to a Base64 string
	var base64Person = Convert.ToBase64String(serializedPerson);

	return base64Person;
}).RequireAuthorization();

app.MapGet("auth/login", (string? returnUrl) => TypedResults.Challenge(new AuthenticationProperties { RedirectUri = returnUrl }))
			.AllowAnonymous();

app.MapGroup("user/").MapUserAPI();

app.MapGrpcService<AppViewServicegRPC>().EnableGrpcWeb();

app.Run();
