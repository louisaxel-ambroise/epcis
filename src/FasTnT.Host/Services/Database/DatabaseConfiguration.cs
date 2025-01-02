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

    public static IServiceCollection AddEpcisStorage(this IServiceCollection services, IConfigurationRoot configuration)
    {
        var connectionString = configuration.GetConnectionString("FasTnT.Database");
        var commandTimeout = configuration.GetValue("FasTnT.Database.CommandTimeout", 30);
        var provider = configuration.GetValue("FasTnT.Database.Provider", nameof(SqlServer));

        if (!Providers.TryGetValue(provider, out var configureAction))
        {
            throw new ArgumentOutOfRangeException("FasTnT.Database.Provider", "Provider is not registered for EPCIS repository");
        }

        configureAction(services, connectionString, commandTimeout);

        return services;
    }
}
