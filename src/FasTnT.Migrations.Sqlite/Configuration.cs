using FasTnT.Application.Relational;
using FasTnT.Application.Relational.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FasTnT.Migrations.Sqlite;

public static class SqliteConfiguration
{
    public static void Configure(IServiceCollection services, string connectionString, int commandTimeout)
    {
        services.AddSingleton<IModelConfiguration, SqliteModelConfiguration>();
        services.AddDbContext<EpcisContext>(o => o.UseSqlite(connectionString, x =>
        {
            x.MigrationsAssembly(typeof(SqliteConfiguration).Assembly.FullName);
            x.CommandTimeout(commandTimeout);
            x.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
        }));
    }
}
