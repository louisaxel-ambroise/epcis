using FasTnT.Application.Database.DataSources;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Masterdata;
using FasTnT.Domain.Model.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FasTnT.Application.Database;

public sealed class EpcisContext : DbContext
{
    public EpcisContext(DbContextOptions<EpcisContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
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

    public IQueryable<MasterdataHierarchy> Hierarchy(string type, string root)
    {
        return Set<MasterdataHierarchy>().Where(x => x.Type == type && x.Root == root);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        EpcisModelConfiguration.Apply(modelBuilder);
    }
}