using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Subscriptions;
using FasTnT.Application.Storage;
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
        services.AddHealthChecks().AddDbContextCheck<EpcisContext>();

        return services;
    }

    public static IServiceCollection AddEpcisSubscriptionServices(this IServiceCollection services)
    {
        services.AddTransient<ISubscriptionRunner, SubscriptionRunner>();
        services.AddSingleton<ISubscriptionService, SubscriptionService>();
        services.AddSingleton<ISubscriptionListener>(ctx => ctx.GetService<ISubscriptionService>());

        return services;
    }
}
