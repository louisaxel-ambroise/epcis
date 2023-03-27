using FasTnT.Application.Services.Subscriptions;

namespace FasTnT.Host.Services.Subscriptions;

public class SubscriptionBackgroundService : BackgroundService
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionBackgroundService(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => _subscriptionService.Run(stoppingToken), stoppingToken);
    }
}
