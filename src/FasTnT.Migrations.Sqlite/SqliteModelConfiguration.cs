using FasTnT.Application.Relational.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FasTnT.Migrations.Sqlite;

internal class SqliteModelConfiguration : RelationalModelConfiguration
{
    public override void Apply(ModelBuilder modelBuilder)
    {
        base.Apply(modelBuilder);

        // Based on: https://github.com/aspnet/EntityFrameworkCore/issues/10784#issuecomment-415769754
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTimeOffset) || p.PropertyType == typeof(DateTimeOffset?));

            foreach (var property in properties)
            {
                modelBuilder.Entity(entityType.Name).Property(property.Name).HasConversion(new DateTimeOffsetToBinaryConverter());
            }
        }
    }
}