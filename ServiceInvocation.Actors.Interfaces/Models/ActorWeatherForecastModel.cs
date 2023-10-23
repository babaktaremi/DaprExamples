namespace ServiceInvocation.Actors.Interfaces.Models;

public record ActorWeatherForecastModel(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}