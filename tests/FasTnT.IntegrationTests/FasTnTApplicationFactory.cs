using FasTnT.Application.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace FasTnT.IntegrationTests;

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

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var configurationValues = new Dictionary<string, string>
        {
            { "FasTnT.Database.Provider", $"Sqlite" },
            { "ConnectionStrings:FasTnT.Database", $"Data Source={_dbName}.db" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationValues)
            .Build();

        builder.UseConfiguration(configuration)
            .ConfigureAppConfiguration(configurationBuilder =>
            {
                configurationBuilder.AddInMemoryCollection(configurationValues);
            });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(svc =>
        {
            using var provider = svc.BuildServiceProvider();
            using var scope = provider.CreateScope();
            using var context = scope.ServiceProvider.GetService<EpcisContext>();

            context.Database.Migrate();
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
