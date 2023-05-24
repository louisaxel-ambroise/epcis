using FasTnT.Application.Database.DataSources;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Masterdata;
using FasTnT.Domain.Model.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FasTnT.Application.Database;

public class EpcisContext : DbContext
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

    internal IQueryable<MasterData> BizLocations => Set<MasterData>().Where(x => x.Type == "urn:epcglobal:epcis:vtype:BusinessLocation");
    internal IQueryable<MasterData> ReadPoints => Set<MasterData>().Where(x => x.Type == "urn:epcglobal:epcis:vtype:ReadPoint");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        EpcisModelConfiguration.Apply(modelBuilder);
    }
}