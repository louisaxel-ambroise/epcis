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
            config.AddInMemoryCollection(new Dictionary<string, string> { { "FasTnT.Database.ApplyMigrations", "true" }, { "FasTnT.Database.Provider", "Sqlite" }, { "ConnectionStrings.FasTnT.Database", "Data Source=fastnt.db;" } });
        });

        if (File.Exists("fastnt.db"))
        {
            File.Delete("fastnt.db");
        }

        return base.CreateHost(builder);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing && File.Exists("fastnt.db"))
        {
            File.Delete("fastnt.db");
        }
    }
}
