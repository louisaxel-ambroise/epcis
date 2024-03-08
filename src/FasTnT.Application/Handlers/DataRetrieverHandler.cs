using FasTnT.Application.Database;
using FasTnT.Application.Database.DataSources.Utils;
using FasTnT.Application.Services.Users;
using FasTnT.Domain;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Masterdata;
using FasTnT.Domain.Model.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FasTnT.Application.Handlers;

public class DataRetrieverHandler(EpcisContext context, ICurrentUser user, IOptions<Constants> constants)
{
    public async Task<List<Event>> QueryEventsAsync(IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken)
    {
        var userParameters = user.DefaultQueryParameters.Union(new []
        {
            new QueryParameter { Name = "orderBy", Values = ["eventTime"] },
            new QueryParameter { Name = "perPage", Values = [constants.Value.MaxEventsReturnedInQuery.ToString()] },
            new QueryParameter { Name = "nextPageToken", Values = ["0"] }
        });

        var maxResults = parameters.LastOrDefault(x => x.Name == "maxEventCount")?.AsInt() ?? constants.Value.MaxEventsReturnedInQuery;
        var eventIds = await context
            .QueryEvents(userParameters.Union(parameters))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (eventIds.Count == 0)
        {
            return [];
        }
        if (eventIds.Count >= maxResults)
        {
            throw new EpcisException(ExceptionType.QueryTooLargeException, "Query returned too many results");
        }

        var events = await context.Set<Event>()
            .WhereIn(x => x.Id, eventIds)
            .ToListAsync(cancellationToken);

        return new (events.OrderBy(e => eventIds.IndexOf(e.Id)));
    }

    public async Task<List<MasterData>> QueryMasterDataAsync(IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken)
    {
        return await context
            .QueryMasterData(parameters)
            .ToListAsync(cancellationToken);
    }
}
