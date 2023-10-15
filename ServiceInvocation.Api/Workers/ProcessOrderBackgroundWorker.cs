using Dapr.Client;

namespace ServiceInvocation.Api.Workers;

public class ProcessOrderBackgroundWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    readonly ILogger<ProcessOrderBackgroundWorker> _logger;

    public ProcessOrderBackgroundWorker(IServiceProvider serviceProvider, ILogger<ProcessOrderBackgroundWorker> logger)
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

            var storedOrder =
                await daprClient.GetStateAsync<OrderReceivedEventModel>(
                    "statestore"
                    , "orderReceivedEvent"
                    ,ConsistencyMode.Strong
                    , cancellationToken: stoppingToken);

            //Simulation of getting a event model from store (Here store is default redis)

            if (storedOrder is not null)
                _logger.LogWarning("Order With ID {orderId} with Order Name {orderName} and description {orderDescription} has been receieved"
                    , storedOrder.OrderId
                    , storedOrder.OrderName
                    , storedOrder.OrderDescription);
            else
                _logger.LogWarning("No Orders To Process");
        }
    }

    internal record OrderReceivedEventModel(int OrderId, string OrderName, string OrderDescription);
}