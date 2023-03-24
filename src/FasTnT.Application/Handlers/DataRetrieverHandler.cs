using FasTnT.Application.Domain.Exceptions;
using FasTnT.Application.Domain.Model.Events;
using FasTnT.Application.Domain.Model.Masterdata;
using FasTnT.Application.Domain.Model.Queries;
using FasTnT.Application.Services.Storage;
using FasTnT.Application.Services.Storage.DataSources.Utils;
using FasTnT.Application.Services.Users;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Handlers;

public class DataRetrieverHandler
{
    private readonly EpcisContext _context;
    private readonly ICurrentUser _user;
    private static readonly IEnumerable<QueryParameter> DefaultEventParameters = new[]
    {
        new QueryParameter { Name = "orderBy", Values = new[]{ "eventTime" } },
        new QueryParameter { Name = "perPage", Values = new[]{ Constants.Instance.MaxEventsReturnedInQuery.ToString() } },
        new QueryParameter { Name = "nextPageToken", Values = new[]{ "0" } }
    };

    public DataRetrieverHandler(EpcisContext context, ICurrentUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<List<Event>> QueryEventsAsync(IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken)
    {
        var userParameters = _user.DefaultQueryParameters.Union(DefaultEventParameters);
        var maxEventCount = parameters.SingleOrDefault(x => x.Name == "maxEventCount")?.AsInt();
        var eventIds = await _context
            .QueryEvents(userParameters.Union(parameters))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (eventIds.Count >= (maxEventCount ?? Constants.Instance.MaxEventsReturnedInQuery))
        {
            throw new EpcisException(ExceptionType.QueryTooLargeException, "Query returned too many results");
        }
        else if (!eventIds.Any())
        {
            return new List<Event>();
        }

        return _context.Set<Event>()
                .Where(x => eventIds.Contains(x.Id))
                .AsEnumerable()
                .OrderBy(x => eventIds.IndexOf(x.Id))
                .ToList();
    }

    public async Task<List<MasterData>> QueryMasterDataAsync(IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken)
    {
        return await _context
            .QueryMasterData(parameters)
            .ToListAsync(cancellationToken);
    }
}
