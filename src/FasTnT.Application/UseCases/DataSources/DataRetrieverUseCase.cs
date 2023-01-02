using FasTnT.Application.Database;
using FasTnT.Application.Services.Users;
using FasTnT.Application.UseCases.DataSources.Utils;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Masterdata;
using FasTnT.Domain.Model.Queries;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.UseCases.DataSources;

public class DataRetrieverUseCase : IDataRetriever
{
    private readonly EpcisContext _context;
    private readonly ICurrentUser _user;

    public DataRetrieverUseCase(EpcisContext context, ICurrentUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<List<Event>> QueryEventsAsync(IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken)
    {
        var maxEventCount = parameters.SingleOrDefault(x => x.Name == "maxEventCount")?.AsFloat();
        var eventIds = await _context
            .QueryEvents(parameters.Union(_user.DefaultQueryParameters))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        return maxEventCount is not null && eventIds.Count >= maxEventCount.Value
            ? throw new EpcisException(ExceptionType.QueryTooLargeException, "Query returned too many results")
            : await _context
                .Set<Event>()
                .AsNoTracking()
                .Where(x => eventIds.Contains(x.Id))
                .ToListAsync(cancellationToken);
    }

    public async Task<List<MasterData>> QueryMasterDataAsync(IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken)
    {
        return await _context
            .QueryMasterData(parameters.Union(_user.DefaultQueryParameters))
            .ToListAsync(cancellationToken);
    }
}
