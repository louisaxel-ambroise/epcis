using FasTnT.Application.Relational;
using FasTnT.Application.Relational.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FasTnT.Migrations.SqlServer;

public static class SqlServerConfiguration
{
    public static void Configure(IServiceCollection services, string connectionString, int commandTimeout)
    {
        services.AddSingleton<IModelConfiguration, SqlServerModelConfiguration>();
        services.AddDbContext<EpcisContext>(o => o.UseSqlServer(connectionString, x =>
        {
            x.MigrationsAssembly(typeof(SqlServerConfiguration).Assembly.FullName);
            x.EnableRetryOnFailure().CommandTimeout(commandTimeout);
            x.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
        }));
    }
}
