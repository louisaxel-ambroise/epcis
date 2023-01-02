using FasTnT.Application.Database;
using FasTnT.Application.Services.Subscriptions;
using FasTnT.Application.UseCases.Captures;
using FasTnT.Application.UseCases.DataSources;
using FasTnT.Application.UseCases.Queries;
using FasTnT.Application.UseCases.Subscriptions;
using FasTnT.Application.UseCases.TopLevelResources;
using Microsoft.Extensions.DependencyInjection;

namespace FasTnT.Application;

public static class EpcisConfiguration
{
    public static IServiceCollection AddEpcisServices(this IServiceCollection services)
    {
        services.AddTransient<DataRetrieverUseCase>();
        services.AddTransient<IDataRetriever, DataRetrieverUseCase>();
        services.AddTransient<IListCaptureRequests, CaptureUseCases>();
        services.AddTransient<ICaptureRequestDetails, CaptureUseCases>();
        services.AddTransient<ICaptureRequest, CaptureUseCases>();
        services.AddTransient<IListQueries, QueriesUseCases>();
        services.AddTransient<IGetQueryDetails, QueriesUseCases>();
        services.AddTransient<IStoreQuery, QueriesUseCases>();
        services.AddTransient<IDeleteQuery, QueriesUseCases>();
        services.AddTransient<ITriggerSubscription, SubscriptionsUseCases>();
        services.AddTransient<IDeleteSubscription, SubscriptionsUseCases>();
        services.AddTransient<IListSubscriptions, SubscriptionsUseCases>();
        services.AddTransient<IRegisterSubscription, SubscriptionsUseCases>();
        services.AddTransient<IGetSubscriptionDetails, SubscriptionsUseCases>();
        services.AddTransient<IListEpcs, TopLevelResourceUseCases>();
        services.AddTransient<IListBizLocations, TopLevelResourceUseCases>();
        services.AddTransient<IListBizSteps, TopLevelResourceUseCases>();
        services.AddTransient<IListEventTypes, TopLevelResourceUseCases>();
        services.AddTransient<IListReadPoints, TopLevelResourceUseCases>();
        services.AddTransient<IListDispositions, TopLevelResourceUseCases>();

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
