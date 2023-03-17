using FasTnT.Application.DataSources;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Masterdata;
using FasTnT.Domain.Model.Queries;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Database;

public class EpcisContext : DbContext
{
    public EpcisContext(DbContextOptions<EpcisContext> options) : base(options)
    {
        SavedChanges += (_, _) => ChangeTracker.Clear();
    }


    public IQueryable<Event> QueryEvents(IEnumerable<QueryParameter> parameters)
    {
        var eventContext = new EventQueryContext(this, parameters);
        var dataset = Set<Event>().AsNoTracking();

        return eventContext.Apply(dataset);
    }

    public IQueryable<MasterData> QueryMasterData(IEnumerable<QueryParameter> parameters)
    {
        var masterdataContext = new MasterDataQueryContext(this, parameters);
        var dataset = Set<MasterData>().AsNoTracking();

        return masterdataContext.Apply(dataset);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        EpcisModelConfiguration.Apply(modelBuilder);
    }
}