using FasTnT.Application.Services.Storage.DataSources;
using FasTnT.Application.Domain.Model.Events;
using FasTnT.Application.Domain.Model.Masterdata;
using FasTnT.Application.Domain.Model.Queries;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Services.Storage;

public class EpcisContext : DbContext
{
    public EpcisContext(DbContextOptions<EpcisContext> options) : base(options)
    {
        ChangeTracker.AutoDetectChangesEnabled = false;
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        SavedChanges += (_, _) => ChangeTracker.Clear();
    }

    public IQueryable<Event> QueryEvents(IEnumerable<QueryParameter> parameters)
    {
        var eventContext = new EventQueryContext(this, parameters);

        return eventContext.ApplyTo(Set<Event>());
    }

    public IQueryable<MasterData> QueryMasterData(IEnumerable<QueryParameter> parameters)
    {
        var masterdataContext = new MasterDataQueryContext(this, parameters);

        return masterdataContext.ApplyTo(Set<MasterData>());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        EpcisModelConfiguration.Apply(modelBuilder);
    }
}