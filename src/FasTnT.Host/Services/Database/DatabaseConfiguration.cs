using FasTnT.Application.Services.Storage;
using FasTnT.Postgres;
using FasTnT.Sqlite;
using FasTnT.SqlServer;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Host.Services.Database;

public static class DatabaseConfiguration
{
    static readonly Dictionary<string, Action<IServiceCollection, string, int>> Providers = new() {
        { nameof(SqlServer), SqlServerProvider.Configure },
        { nameof(Postgres), PostgresProvider.Configure },
        { nameof(Sqlite), SqliteProvider.Configure }
    };

    public static IServiceCollection AddEpcisStorage(this IServiceCollection services, ConfigurationManager configuration)
    {
        var connectionString = configuration.GetConnectionString("FasTnT.Database");
        var commandTimeout = configuration.GetValue("FasTnT.Database.CommandTimeout", 30);
        var provider = configuration.GetValue("FasTnT.Database.Provider", nameof(SqlServer));

        if (!Providers.TryGetValue(provider, out var configureAction))
        {
            throw new ArgumentException("Provider is not registered for EPCIS repository", "FasTnT.Database.Provider");
        }
        
        configureAction(services, connectionString, commandTimeout);

        return services;
    }

    public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder application)
    {
        using var scope = application.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<EpcisContext>();

        context.Database.Migrate();

        return application;
    }
}
