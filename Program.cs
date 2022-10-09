using RazerPagesPokedex;
using RazerPagesPokedex.PokemonApiClient;
using RazorPagesPokedex.BackgroundServices;
using RazorPagesPokedex.Extensions;
using RazorPagesPokedex.PokemonApiClient;
using RazorPagesPokedex.Services;
using Serilog;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

//config
builder.Configure<ServiceConfig>();
builder.Configure<JsonSerializerOptions>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddHttpClient();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSingleton<IPokemonDataClient, PokemonDataClient>();
builder.Services.AddSingleton<IPokemonVmService, PokemonVmService>();
builder.Services.AddHostedService<CacheWarmingBackgroundService>();
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
