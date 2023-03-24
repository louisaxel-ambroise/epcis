using FasTnT.Application.Storage.DataSources;
using FasTnT.Application.Domain.Model.Events;
using FasTnT.Application.Domain.Model.Masterdata;
using FasTnT.Application.Domain.Model.Queries;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Storage;

public class EpcisContext : DbContext
{
    public EpcisContext(DbContextOptions<EpcisContext> options) : base(options)
    {
        ChangeTracker.AutoDetectChangesEnabled = false;
        SavedChanges += (_, _) => ChangeTracker.Clear();
    }

    public IQueryable<Event> QueryEvents(IEnumerable<QueryParameter> parameters)
    {
        var eventContext = new EventQueryContext(this, parameters);
        var dataset = Set<Event>().AsNoTracking();

        return eventContext.ApplyTo(dataset);
    }

    public IQueryable<MasterData> QueryMasterData(IEnumerable<QueryParameter> parameters)
    {
        var masterdataContext = new MasterDataQueryContext(this, parameters);
        var dataset = Set<MasterData>().AsNoTracking();

        return masterdataContext.ApplyTo(dataset);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        EpcisModelConfiguration.Apply(modelBuilder);
    }
}