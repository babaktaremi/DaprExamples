using Bogus;
using Dapr.Client;

namespace ServiceInvocation.UI.Workers;

public class StoreOrderBackgroundWorker:BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<StoreOrderBackgroundWorker> _logger;
    public StoreOrderBackgroundWorker(IServiceProvider serviceProvider, ILogger<StoreOrderBackgroundWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var period = TimeSpan.FromSeconds(15);
        using var timer = new PeriodicTimer(period);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            await using var serviceScope = _serviceProvider.CreateAsyncScope();

            var daprClient = serviceScope.ServiceProvider.GetRequiredService<DaprClient>();
            var faker = new Faker();


            //Simulation of saving state in a state store (Here default is redis)
            var order = new OrderReceivedEventModel(Random.Shared.Next(1, 1200)
                , faker.Commerce.ProductName()
                , faker.Commerce.ProductDescription());

            await daprClient.SaveStateAsync("statestore"
                , "orderReceivedEvent"
                , order
                , cancellationToken: stoppingToken);
            _logger.LogWarning("Order Item Stored For Process With Id {orderId}",order.OrderId);
        }
    }

    internal record OrderReceivedEventModel(int OrderId, string OrderName, string OrderDescription);
}