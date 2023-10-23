using Dapr.Actors.Runtime;
using ServiceInvocation.Actors.Interfaces.Interfaces;
using ServiceInvocation.Actors.Interfaces.Models;

namespace ServiceInvocation.Api.ActorImplementations;

public class WeatherActor:Actor,IWeatherActor
{
 readonly  ILogger<WeatherActor> _logger;
 private  ActorWeatherForecastModel _weatherForecastModel;
    public WeatherActor(ActorHost host, ILogger<WeatherActor> logger) : base(host)
    {
    
        _logger = logger;
    }

    protected override async Task OnActivateAsync()
    {
        await base.OnActivateAsync();
        _logger.LogWarning
            ($"Actor Activation Completed: {base.Id.ToString()}");

        _weatherForecastModel = new ActorWeatherForecastModel(DateTime.Now.AddDays(
                new Random().Next(10))
            , new Random().Next(40)
            , $"Actor Responsible For This Summery is {base.Id.ToString()}");
    }

    protected override Task OnActorMethodFailedAsync(ActorMethodContext actorMethodContext, Exception e)
    {
        _logger.LogError("Actor Activation Failed!");

        return base.OnActorMethodFailedAsync(actorMethodContext, e);
    }

    public Task<ActorWeatherForecastModel> GetWeatherForecastAsync()
    {
        return Task.FromResult(_weatherForecastModel);
    }
}