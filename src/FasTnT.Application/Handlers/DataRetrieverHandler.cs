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

public class DataRetrieverHandler
{
    private readonly EpcisContext _context;
    private readonly ICurrentUser _user;
    private readonly Constants _constants;

    public DataRetrieverHandler(EpcisContext context, ICurrentUser user, IOptions<Constants> constants)
    {
        _context = context;
        _user = user;
        _constants = constants.Value;
    }

    public async Task<List<Event>> QueryEventsAsync(IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken)
    {
        var userParameters = _user.DefaultQueryParameters.Union(new []
        {
            new QueryParameter { Name = "orderBy", Values = new[]{ "eventTime" } },
            new QueryParameter { Name = "perPage", Values = new[]{ _constants.MaxEventsReturnedInQuery.ToString() } },
            new QueryParameter { Name = "nextPageToken", Values = new[]{ "0" } }
        });

        var maxEventCount = parameters.SingleOrDefault(x => x.Name == "maxEventCount")?.AsInt();
        var eventIds = await _context
            .QueryEvents(userParameters.Union(parameters))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (!eventIds.Any())
        {
            return new List<Event>();
        }
        if (eventIds.Count >= (maxEventCount ?? _constants.MaxEventsReturnedInQuery))
        {
            throw new EpcisException(ExceptionType.QueryTooLargeException, "Query returned too many results");
        }

        var events = await _context.Set<Event>()
            .Where(x => eventIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        return events.OrderBy(e => eventIds.IndexOf(e.Id)).ToList();
    }

    public async Task<List<MasterData>> QueryMasterDataAsync(IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken)
    {
        return await _context
            .QueryMasterData(parameters)
            .ToListAsync(cancellationToken);
    }
}
