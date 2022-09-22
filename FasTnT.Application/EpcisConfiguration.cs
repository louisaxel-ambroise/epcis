using FasTnT.Application.Services.Queries;
using FasTnT.Application.Services.Subscriptions;
using FasTnT.Application.Store;
using FasTnT.Application.Store.Configuration;
using FasTnT.Application.UseCases.Captures;
using FasTnT.Application.UseCases.ListTopLevelResources;
using FasTnT.Application.UseCases.Queries;
using FasTnT.Application.UseCases.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FasTnT.Application;

public static class EpcisConfiguration
{
    public static IServiceCollection AddEpcisServices(this IServiceCollection services) => services.AddEpcisServices(null);

    public static IServiceCollection AddEpcisServices(this IServiceCollection services, Action<EpcisOptions> configure)
    {
        var options = new EpcisOptions();
        if (configure is not null)
        {
            configure(options);
        }

        services.AddDbContext<EpcisContext>(opt => opt.UseSqlServer(options.ConnectionString, opt => opt.MigrationsAssembly("FasTnT.Migrations.SqlServer").EnableRetryOnFailure().CommandTimeout(options.CommandTimeout)), ServiceLifetime.Transient);

        services.AddSingleton<IEpcisDataSource, SimpleEventQuery>();
        services.AddSingleton<IEpcisDataSource, SimpleMasterDataQuery>();

        services.AddTransient<IListCaptureRequestsHandler, CaptureUseCasesHandler>();
        services.AddTransient<ICaptureRequestDetailsHandler, CaptureUseCasesHandler>();
        services.AddTransient<ICaptureRequestHandler, CaptureUseCasesHandler>();
        services.AddTransient<IListQueriesHandler, QueriesUseCasesHandler>();
        services.AddTransient<IGetQueryDetailsHandler, QueriesUseCasesHandler>();
        services.AddTransient<IStoreQueryHandler, QueriesUseCasesHandler>();
        services.AddTransient<IDeleteQueryHandler, QueriesUseCasesHandler>();
        services.AddTransient<IExecuteQueryHandler, QueriesUseCasesHandler>();
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

        services.AddScoped<IncrementGenerator.Identity>();
        services.AddScoped(options.CurrentUser);
        services.AddScoped(options.UserProvider);

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
}
