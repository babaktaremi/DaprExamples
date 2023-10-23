using Dapr.Actors;
using ServiceInvocation.Actors.Interfaces.Models;

namespace ServiceInvocation.Actors.Interfaces.Interfaces;

public interface IWeatherActor:IActor
{
    Task<ActorWeatherForecastModel> GetWeatherForecastAsync();
}