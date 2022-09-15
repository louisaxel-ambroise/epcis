using FasTnT.Application.Services.Queries;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Services.Users.Providers;
using FasTnT.Application.Store;
using FasTnT.Application.Store.Configuration;
using FasTnT.Application.UseCases.Captures;
using FasTnT.Application.UseCases.CustomQueries;
using FasTnT.Application.UseCases.ListTopLevelResources;
using FasTnT.Application.UseCases.StandardQueries;
using FasTnT.Application.UseCases.Subscriptions;
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

        services.AddSqlServer<EpcisContext>(options.ConnectionString, opt => opt.MigrationsAssembly("FasTnT.Migrations.SqlServer").EnableRetryOnFailure().CommandTimeout(options.CommandTimeout));

        services.AddSingleton<IStandardQuery, SimpleEventQuery>();
        services.AddSingleton<IStandardQuery, SimpleMasterDataQuery>();

        services.AddTransient<IDeleteCustomQueryHandler, CustomQueriesUseCasesHandler>();
        services.AddTransient<IStoreCustomQueryHandler, CustomQueriesUseCasesHandler>();
        services.AddTransient<IListCustomQueriesHandler, CustomQueriesUseCasesHandler>();
        services.AddTransient<IGetCustomQueryDetailsHandler, CustomQueriesUseCasesHandler>();
        services.AddTransient<IExecuteCustomQueryHandler, CustomQueriesUseCasesHandler>();
        services.AddTransient<IListCaptureRequestsHandler, CaptureUseCasesHandler>();
        services.AddTransient<ICaptureRequestDetailsHandler, CaptureUseCasesHandler>();
        services.AddTransient<ICaptureRequestHandler, CaptureUseCasesHandler>();
        services.AddTransient<IGetStandardQueryNamesHandler, StandardQueriesUseCasesHandler>();
        services.AddTransient<IExecuteStandardQueryHandler, StandardQueriesUseCasesHandler>();
        services.AddTransient<ITriggerSubscriptionHandler, SubscriptionsUseCasesHandler>();
        services.AddTransient<IDeleteSubscriptionHandler, SubscriptionsUseCasesHandler>();
        services.AddTransient<IListSubscriptionsHandler, SubscriptionsUseCasesHandler>();
        services.AddTransient<ICustomQuerySubscriptionHandler, SubscriptionsUseCasesHandler>();
        services.AddTransient<IStandardQuerySubscriptionHandler, SubscriptionsUseCasesHandler>();
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
}

public class EpcisOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public int CommandTimeout { get; set; } = 60;
    public Func<IServiceProvider, ICurrentUser> CurrentUser { get; set; } = _ => null;
    public Func<IServiceProvider, IUserProvider> UserProvider { get; set; } = svc => new UserProvider(svc.GetService<EpcisContext>());
}