using Google.Protobuf;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using UICrafter;
using UICrafter.Components;
using UICrafter.Core.Models;
using UICrafter.Core.Repository;
using UICrafter.Core.Utility;
using UICrafter.Repository;
using UICrafter.Services;
using UICrafter.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents()
	.AddInteractiveWebAssemblyComponents();

builder.Services.AddHttpClient();
builder.Services.AddScoped<IHttpClientProvider, HttpClientProvider>();

//gRPC
builder.Services.AddGrpc();

// Swagger setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Repository
builder.Services.AddScoped<IAppViewRepository, AppViewRepository>();

// Database setup
builder.Services.AddDbContext<ApplicationDbContext>(options =>
		options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseWebAssemblyDebugging();
	app.UseSwagger();
	app.UseSwaggerUI();
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


app.MapGet("api/prototest", () =>
{
	// Create a new Person object
	var person = new Person { Email = "muppet@muppet.dk", Name = "Muppet" };

	// Serialize the person object to a binary byte array
	var serializedPerson = person.ToByteArray();

	// Convert the byte array to a Base64 string
	var base64Person = Convert.ToBase64String(serializedPerson);

	return base64Person;
});

app.MapGrpcService<AppViewServicegRPC>().EnableGrpcWeb();

app.Run();
