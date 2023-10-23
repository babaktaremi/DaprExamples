using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;
using ServiceInvocation.Actors.Interfaces.Interfaces;
using ServiceInvocation.UI.Workers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages().AddDapr();
builder.Services.AddDaprClient();
//builder.Services.AddHostedService<StoreOrderBackgroundWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

//For Using Async Messaging
app.UseCloudEvents();
app.MapSubscribeHandler();

app.MapGet("/InvokeActor", async (string actorId) =>
{
    var actorType = "WeatherActor";

    // An ActorId uniquely identifies an actor instance
    // If the actor matching this id does not exist, it will be created
    var actorIdentifier = new ActorId(actorId);


    var actorProxy =
        ActorProxy.Create<IWeatherActor>(actorIdentifier, actorType, new ActorProxyOptions() { UseJsonSerialization = true });
  ;

    var result = await actorProxy.GetWeatherForecastAsync();

    return Results.Ok(result);
});

app.Run();
