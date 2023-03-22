using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace FasTnT.IntegrationTests;

internal class FasTnTApplicationFactory : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string> { { "FasTnT.Database.ApplyMigrations", "false" }, { "FasTnT.Database.Provider", "Sqlite" }, { "ConnectionStrings.FasTnT.Database", "Data Source=fastnt.db;" } });
        });

        return base.CreateHost(builder);
    }
}
