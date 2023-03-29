using FasTnT.Application.Database;
using FasTnT.Application.Handlers;
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
}
