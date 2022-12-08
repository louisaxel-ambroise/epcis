using FasTnT.Application;
using FasTnT.Application.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FasTnT.CosmosDb;

public static class CosmosDbProvider
{
    public static void Configure(IServiceCollection services, string connectionString, int commandTimeout)
    {
        services.AddSingleton<IModelConfiguration, CosmosModelConfiguration>();
        services.AddDbContext<EpcisContext>(o => o.UseCosmos(connectionString, nameof(EpcisContext), c =>
        {
            c.RequestTimeout(TimeSpan.FromSeconds(commandTimeout));
        }));
    }
}
