using FasTnT.Application.Database;
using FasTnT.Application.Handlers.DataSources.Contexts;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Masterdata;
using FasTnT.Domain.Model.Queries;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Handlers.DataSources.Utils;

public static class EpcisContextExtensions
{
    public static IQueryable<Event> QueryEvents(this EpcisContext context, IEnumerable<QueryParameter> parameters)
    {
        var queryContext = new EventQueryContext(context, parameters);
        var dataset = context.Set<Event>().AsNoTrackingWithIdentityResolution();

        return queryContext.Apply(dataset);
    }

    public static IQueryable<MasterData> QueryMasterData(this EpcisContext context, IEnumerable<QueryParameter> parameters)
    {
        var queryContext = new MasterDataQueryContext(context, parameters);
        var dataset = context.Set<MasterData>().AsNoTrackingWithIdentityResolution();

        return queryContext.Apply(dataset);
    }
}
