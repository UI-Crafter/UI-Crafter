using Google.Protobuf;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using MudBlazor.Services;
using UICrafter;
using UICrafter.Components;
using UICrafter.Core.Models;
using UICrafter.Core.Repository;
using UICrafter.Core.Utility;
using UICrafter.EntityConfigurations;
using UICrafter.Repository;
using UICrafter.Services;
using UICrafter.Utility;

var builder = WebApplication.CreateBuilder(args);

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
	//builder.Services.AddGrpcSwagger().AddSwaggerGen(c =>
	//{
	//	c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

	//	// Add OAuth2 definition
	//	c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
	//	{
	//		Type = SecuritySchemeType.OAuth2,
	//		Flows = new OpenApiOAuthFlows
	//		{
	//			AuthorizationCode = new OpenApiOAuthFlow
	//			{
	//				AuthorizationUrl = new Uri($"https://{builder.Configuration["AzureAd:Domain"]}/{builder.Configuration["AzureAd:TenantId"]}/{builder.Configuration["AzureAd:SignUpSignInPolicyId"]}/oauth2/v2.0/authorize"),
	//				TokenUrl = new Uri($"https://{builder.Configuration["AzureAd:Domain"]}/{builder.Configuration["AzureAd:TenantId"]}/{builder.Configuration["AzureAd:SignUpSignInPolicyId"]}/oauth2/v2.0/token"),
	//				Scopes = new Dictionary<string, string>
	//			{
	//					{ "openid", "45698317-ca1e-4595-a069-3dad3bce31a6" }
	//			}
	//			}
	//		}
	//	});

	//	// Apply security globally to all operations
	//	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	//{
	//	{
	//		new OpenApiSecurityScheme
	//		{
	//			Reference = new OpenApiReference
	//			{
	//				Type = ReferenceType.SecurityScheme,
	//				Id = "oauth2"
	//			}
	//		},
	//		new[] {"openid"}
	//	}
	//});
	//});
}
else
{
	builder.Services.AddGrpc();
}

//builder.Services.AddGrpc().AddJsonTranscoding();

//builder.Services.AddGrpcSwagger().AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "gRPC transcoding", Version = "v1" }));



// Swagger setup
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();



// Repository
builder.Services.AddScoped<IAppViewRepository, AppViewRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Database setup
builder.Services.AddDbContext<ApplicationDbContext>(options =>
		options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

// Auth
builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration);
builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, OpenIdConnectConfiguration.Configure);
builder.Services.AddScoped<AuthenticationStateProvider, PersistingAuthenticationStateProvider>();
builder.Services.AddAuthorization();
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
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
	});
	//app.UseSwaggerUI(c =>
	//{
	//	c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");

	//	// Enable OAuth2 authentication in Swagger UI
	//	c.OAuthClientId(builder.Configuration["AzureAd:ClientId"]);
	//	c.OAuthUsePkce(); // PKCE is recommended for security
	//	c.OAuthScopes("openid");
	//	c.OAuthAdditionalQueryStringParams(new Dictionary<string, string>
	//{
	//	{ "p", builder.Configuration["AzureAd:SignUpSignInPolicyId"] }
	//});
	//});

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

app.MapGrpcService<AppViewServicegRPC>().EnableGrpcWeb();

app.Run();
