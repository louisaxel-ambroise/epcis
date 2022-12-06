using FasTnT.Application.Relational.Configuration;
using FasTnT.Domain.Model.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FasTnT.Migrations.Sqlite;

internal class SqliteModelConfiguration : BaseRelationalModelConfiguration
{
    public override void Apply(ModelBuilder modelBuilder)
    {
        base.Apply(modelBuilder);

        modelBuilder.Entity<Event>().Property<long>("Id").ValueGeneratedOnAdd();

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