using FasTnT.Application.Database;
using FasTnT.Application.UseCases.DataSources.Contexts;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Masterdata;
using FasTnT.Domain.Model.Queries;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.UseCases.DataSources.Utils;

public static class Extensions
{
    public static IQueryable<Event> QueryEvents(this EpcisContext context, IEnumerable<QueryParameter> parameters)
    {
        var queryContext = new EventQueryContext(context, parameters);

        return queryContext.Apply(context.Set<Event>().AsNoTracking());
    }

    public static IQueryable<MasterData> QueryMasterData(this EpcisContext context, IEnumerable<QueryParameter> parameters)
    {
        var queryContext = new MasterDataQueryContext(context, parameters);

        return queryContext.Apply(context.Set<MasterData>().AsNoTracking());
    }
}
