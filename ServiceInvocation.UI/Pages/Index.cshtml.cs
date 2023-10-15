using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ServiceInvocation.UI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly DaprClient _daprClient;

        public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary);

        public IndexModel(ILogger<IndexModel> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }

        [BindProperty] public List<WeatherForecast> WeatherForecasts { get; set; }


        public async Task OnGetAsync()
        {
            //Direct invocation (Service Discovery)
           WeatherForecasts= await _daprClient
               .InvokeMethodAsync<List<WeatherForecast>>(HttpMethod.Get
                   , "weather-api"
                   , "weatherforecast");


            //Message Bus Invocation Using Pub/Sub Pattern

            await _daprClient.PublishEventAsync("pubsub", "WeatherInfoRequested", new WeatherInfoRequest("UI", DateTime.Now));
        }
    }

    public record WeatherInfoRequest(string ServiceName, DateTime RequestDate);
}