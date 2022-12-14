using FasTnT.Application.Database;
using FasTnT.Postgres;
using FasTnT.Sqlite;
using FasTnT.SqlServer;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Host.Services.Database;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddEpcisStorage(this IServiceCollection services, ConfigurationManager configuration)
    {
        var connectionString = configuration.GetConnectionString("FasTnT.Database");
        var commandTimeout = configuration.GetValue("FasTnT.Database.CommandTimeout", 30);
        var provider = configuration.GetValue("FasTnT.Database.Provider", nameof(SqlServer));
        var configureAction = GetConfigurationAction(provider);

        configureAction(services, connectionString, commandTimeout);

        return services;
    }

    private static Action<IServiceCollection, string, int> GetConfigurationAction(string provider)
    {
        return provider switch
        {
            nameof(SqlServer) => SqlServerProvider.Configure,
            nameof(Postgres) => PostgresProvider.Configure,
            nameof(Sqlite) => SqliteProvider.Configure,
            _ => throw new Exception($"Unsupported provider: {provider}")
        };
    }

    public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder application)
    {
        using var scope = application.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<EpcisContext>();

        context.Database.Migrate();

        return application;
    }
}
