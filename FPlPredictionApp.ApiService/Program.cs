using FPlPredictionApp.ApiService;
using Carter;
using System.Net;
using System.Net.Security;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Configure HttpClient with SSL certificate handling
builder.Services.AddHttpClient<FootballDataApiClient>((serviceProvider, client) =>
{
    // Configuration is handled in the constructor of FootballDataApiClient
})
.ConfigurePrimaryHttpMessageHandler(() => 
{
    // Check if we're in a test environment (CI) where certificates might be an issue
    var isInTestEnv = builder.Environment.IsDevelopment() || 
                      !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"));
    
    return new HttpClientHandler
    {
        // Bypass SSL certificate validation in test environments
        ServerCertificateCustomValidationCallback = isInTestEnv 
            ? (message, cert, chain, errors) => true 
            : null
    };
});

builder.Services.AddCarter();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// Remove old premierleague endpoints, now handled by Carter
app.MapDefaultEndpoints();

app.MapCarter();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
