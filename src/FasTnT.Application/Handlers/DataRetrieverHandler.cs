using FasTnT.Application.Database;
using FasTnT.Application.Handlers.DataSources.Utils;
using FasTnT.Application.Services.Users;
using FasTnT.Domain;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Masterdata;
using FasTnT.Domain.Model.Queries;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Handlers;

public class DataRetrieverHandler
{
    private readonly EpcisContext _context;
    private readonly ICurrentUser _user;

    public DataRetrieverHandler(EpcisContext context, ICurrentUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<List<Event>> QueryEventsAsync(IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken)
    {
        var maxEventCount = parameters.SingleOrDefault(x => x.Name == "maxEventCount")?.AsInt();
        var eventIds = await _context
            .QueryEvents(parameters.Union(_user.DefaultQueryParameters))
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
            .QueryMasterData(parameters.Union(_user.DefaultQueryParameters))
            .ToListAsync(cancellationToken);
    }
}
