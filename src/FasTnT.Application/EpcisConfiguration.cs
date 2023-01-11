using FasTnT.Application.Database;
using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Subscriptions;
using Microsoft.Extensions.DependencyInjection;

namespace FasTnT.Application;

public static class EpcisConfiguration
{
    public static IServiceCollection AddEpcisServices(this IServiceCollection services)
    {
        services.AddTransient<DataRetrieverHandler>();
        services.AddTransient<DataRetrieverHandler>();
        services.AddTransient<CaptureHandler>();
        services.AddTransient<QueriesHandler>();
        services.AddTransient<SubscriptionsHandler>();
        services.AddTransient<TopLevelResourceHandler>();

        if (!services.Any(x => typeof(ISubscriptionListener).IsAssignableFrom(x.ServiceType)))
        {
            services.AddSingleton<ISubscriptionListener, NoOpSubscriptionListener>();
        }

        services.AddHealthChecks().AddDbContextCheck<EpcisContext>();

        return services;
    }

    public static IServiceCollection AddEpcisSubscriptionServices(this IServiceCollection services, params IResultSender[] resultSenders)
    {
        services.AddTransient<ISubscriptionRunner, SubscriptionRunner>();
        services.AddSingleton<ISubscriptionService, SubscriptionService>();
        services.AddSingleton<IEnumerable<IResultSender>>(resultSenders);

        services.AddSingleton<ISubscriptionListener>(ctx => ctx.GetService<ISubscriptionService>());

        return services;
    }

    private class NoOpSubscriptionListener : ISubscriptionListener
    {
    }
}
