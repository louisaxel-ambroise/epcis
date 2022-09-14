using FasTnT.Application.Services;
using FasTnT.Application.Services.Capture;
using FasTnT.Application.Services.Queries;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Services.Users.Providers;
using FasTnT.Application.Store;
using FasTnT.Application.Store.Configuration;
using FasTnT.Application.UseCases.CaptureRequestDetails;
using FasTnT.Application.UseCases.DeleteCustomQuery;
using FasTnT.Application.UseCases.DeleteSubscription;
using FasTnT.Application.UseCases.ExecuteCustomQuery;
using FasTnT.Application.UseCases.ExecuteStandardQuery;
using FasTnT.Application.UseCases.GetCustomQueryDetails;
using FasTnT.Application.UseCases.GetStandardQueryNames;
using FasTnT.Application.UseCases.ListCustomQueries;
using FasTnT.Application.UseCases.ListSubscriptions;
using FasTnT.Application.UseCases.StoreCustomQuery;
using FasTnT.Application.UseCases.StoreCustomQuerySubscription;
using FasTnT.Application.UseCases.StoreStandardQuerySubscription;
using FasTnT.Application.UseCases.TriggerSubscription;
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

        services.AddSingleton<IStandardQuery, SimpleEventQuery>();
        services.AddSingleton<IStandardQuery, SimpleMasterDataQuery>();
        services.AddTransient<IListCustomQueriesHandler, ListCustomQueriesHandler>();
        services.AddTransient<IGetCustomQueryDetailsHandler, GetCustomQueryDetailsHandler>();
        services.AddTransient<IStoreEpcisDocumentHandler, StoreEpcisDocumentHandler>();
        services.AddTransient<IExecuteCustomQueryHandler, ExecuteCustomQueryHandler>();
        services.AddTransient<IGetStandardQueryNamesHandler, GetStandardQueryNamesHandler>();
        services.AddTransient<IExecuteStandardQueryHandler, ExecuteStandardQueryHandler>();
        services.AddTransient<IDeleteCustomQueryHandler, DeleteCustomQueryHandler>();
        services.AddTransient<IStoreCustomQueryHandler, StoreCustomQueryHandler>();
        services.AddTransient<ITriggerSubscriptionHandler, TriggerSubscriptionHandler>();
        services.AddTransient<IDeleteSubscriptionHandler, DeleteSubscriptionHandler>();
        services.AddTransient<IListSubscriptionsHandler, ListSubscriptionsHandler>();
        services.AddTransient<IStoreCustomQuerySubscriptionHandler, StoreCustomQuerySubscriptionHandler>();
        services.AddTransient<IStoreStandardQuerySubscriptionHandler, StoreStandardQuerySubscriptionHandler>();
        services.AddTransient<ICaptureRequestDetailsHandler, CaptureRequestDetailsHandler>();
        services.AddScoped<IncrementGenerator.Identity>();
        services.AddScoped(options.CurrentUser);
        services.AddScoped(options.UserProvider);
        services.AddSqlServer<EpcisContext>(options.ConnectionString, opt => opt.MigrationsAssembly("FasTnT.Migrations.SqlServer").EnableRetryOnFailure().CommandTimeout(options.CommandTimeout));

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