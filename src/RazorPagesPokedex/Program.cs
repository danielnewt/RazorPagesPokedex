using MediatR;
using PokeApiClient.DependencyInjection;
using PokeApiClient.Options;
using RazorPagesPokedex.Extensions;
using Serilog;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

//config
builder.Configure<JsonSerializerOptions>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddMediatR(typeof(Program));

builder.Services.AddPokeApiClient(options => builder.Configuration.GetSection(nameof(PokeApiOptions)).Bind(options));

builder.Host.UseSerilog((hostContext, logConfig) => logConfig.ReadFrom.Configuration(hostContext.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
