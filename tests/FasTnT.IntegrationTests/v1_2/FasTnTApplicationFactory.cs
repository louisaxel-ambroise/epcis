using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace FasTnT.IntegrationTests.v1_2;

internal class FasTnTApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName;

    public FasTnTApplicationFactory(string dbName)
    {
        _dbName = dbName;

        if (File.Exists($"{_dbName}.db"))
        {
            File.Delete($"{_dbName}.db");
        }
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string> { { "FasTnT.Database.ApplyMigrations", "true" }, { "FasTnT.Database.Provider", "Sqlite" }, { "ConnectionStrings:FasTnT.Database", $"Data Source={_dbName}.db;" } });
        });

        return base.CreateHost(builder);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing && File.Exists($"{_dbName}.db"))
        {
            File.Delete($"{_dbName}.db");
        }
    }
}
