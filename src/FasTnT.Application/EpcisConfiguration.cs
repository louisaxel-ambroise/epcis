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
        services.AddTransient<DataRetrieveUseCaseHandler>();
        services.AddTransient<IDataRetrieveHandler, DataRetrieveUseCaseHandler>();
        services.AddTransient<IListCaptureRequestsHandler, CaptureUseCasesHandler>();
        services.AddTransient<ICaptureRequestDetailsHandler, CaptureUseCasesHandler>();
        services.AddTransient<ICaptureRequestHandler, CaptureUseCasesHandler>();
        services.AddTransient<IListQueriesHandler, QueriesUseCasesHandler>();
        services.AddTransient<IGetQueryDetailsHandler, QueriesUseCasesHandler>();
        services.AddTransient<IStoreQueryHandler, QueriesUseCasesHandler>();
        services.AddTransient<IDeleteQueryHandler, QueriesUseCasesHandler>();
        services.AddTransient<ITriggerSubscriptionHandler, SubscriptionsUseCasesHandler>();
        services.AddTransient<IDeleteSubscriptionHandler, SubscriptionsUseCasesHandler>();
        services.AddTransient<IListSubscriptionsHandler, SubscriptionsUseCasesHandler>();
        services.AddTransient<IRegisterSubscriptionHandler, SubscriptionsUseCasesHandler>();
        services.AddTransient<IGetSubscriptionDetailsHandler, SubscriptionsUseCasesHandler>();
        services.AddTransient<IListEpcsHandler, TopLevelResourceUseCasesHandler>();
        services.AddTransient<IListBizLocationsHandler, TopLevelResourceUseCasesHandler>();
        services.AddTransient<IListBizStepsHandler, TopLevelResourceUseCasesHandler>();
        services.AddTransient<IListEventTypesHandler, TopLevelResourceUseCasesHandler>();
        services.AddTransient<IListReadPointsHandler, TopLevelResourceUseCasesHandler>();
        services.AddTransient<IListDispositionsHandler, TopLevelResourceUseCasesHandler>();

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
