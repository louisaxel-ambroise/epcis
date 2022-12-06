using FasTnT.Application.Relational;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Host.Extensions;

public static class DatabaseMigrator
{
    public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder application)
    {
        using var scope = application.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<EpcisContext>();

        context.Database.EnsureCreated();

        if (context.Database.IsRelational())
        {
            context.Database.Migrate();
        }

        return application;
    }
}
