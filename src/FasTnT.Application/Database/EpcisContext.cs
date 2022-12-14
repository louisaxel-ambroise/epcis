using FasTnT.Application.Services.DataSources;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Database;

public class EpcisContext : DbContext
{
    public EpcisContext(DbContextOptions<EpcisContext> options) : base(options)
    {
    }

    public EventDataSource QueryEvents() => new(this);
    public VocabularyDataSource QueryVocabulary() => new(this);

    public IEpcisDataSource DataSource(string dataSource)
    {
        return dataSource switch
        {
            nameof(EventDataSource) => QueryEvents(),
            nameof(VocabularyDataSource) => QueryVocabulary(),
            _ => throw new Exception()
        };
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        EpcisModelConfiguration.Apply(modelBuilder);
    }
}