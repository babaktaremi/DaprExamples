using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.HttpLogging;
using ServiceInvocation.Actors.Interfaces.Interfaces;
using ServiceInvocation.Api.ActorImplementations;
using ServiceInvocation.Api.Workers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDaprClient();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<ProcessOrderBackgroundWorker>();
builder.Services.AddW3CLogging(opt =>
{
    opt.LoggingFields = W3CLoggingFields.All;
    opt.FlushInterval= TimeSpan.FromSeconds(15);
});

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});


builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<WeatherActor>();
    options.UseJsonSerialization = true;
    options.JsonSerializerOptions = new JsonSerializerOptions()
        { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    options.ActorIdleTimeout = TimeSpan.FromMinutes(1);
    options.ActorScanInterval = TimeSpan.FromSeconds(30);
    options.DrainOngoingCallTimeout = TimeSpan.FromSeconds(30);
    options.DrainRebalancedActors = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

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
    .WithName("GetWeatherForecast")
    .WithOpenApi();


app.MapPost("/WeatherInfoRequest", (WeatherInfoRequest request, ILogger<Program> logger) =>
{
    logger.LogWarning("Weather Info Requested By Service {service} at time {time}", request.ServiceName, request.RequestDate);

    return Results.Accepted();

}).WithTopic("pubsub", "WeatherInfoRequested");


app.UseCloudEvents();
app.MapSubscribeHandler();
app.UseW3CLogging();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapActorsHandlers();
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

internal record WeatherInfoRequest(string ServiceName, DateTime RequestDate);