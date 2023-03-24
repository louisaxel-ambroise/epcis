using FasTnT.Application.Services.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FasTnT.SqlServer;

public static class SqlServerProvider
{
    public static void Configure(IServiceCollection services, string connectionString, int commandTimeout)
    {
        services.AddDbContextPool<EpcisContext>(o => o.UseSqlServer(connectionString, x =>
        {
            x.MigrationsAssembly(typeof(SqlServerProvider).Assembly.FullName);
            x.EnableRetryOnFailure().CommandTimeout(commandTimeout);
            x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        }));
    }
}
