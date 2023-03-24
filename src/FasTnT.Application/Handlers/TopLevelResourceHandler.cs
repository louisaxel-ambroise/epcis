using FasTnT.Application.Domain.Model.Events;
using FasTnT.Application.Domain.Model.Queries;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Storage;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FasTnT.Application.Handlers;

public class TopLevelResourceHandler
{
    private readonly EpcisContext _context;
    private readonly ICurrentUser _user;

    public TopLevelResourceHandler(EpcisContext context, ICurrentUser user)
    {
        _context = context;
        _user = user;
    }

    public static IEnumerable<string> ListEventTypes(Pagination pagination)
    {
        return Enum.GetValues<EventType>()
            .Where(x => !new[] { EventType.None, EventType.QuantityEvent }.Contains(x))
            .Skip(pagination.StartFrom)
            .Take(pagination.PerPage)
            .Select(x => x.ToString());
    }

    public async Task<IEnumerable<string>> ListEpcs(Pagination pagination, CancellationToken cancellationToken)
    {
        var epcs = await _context
            .QueryEvents(_user.DefaultQueryParameters)
            .SelectMany(x => x.Epcs.Select(x => x.Id))
            .Distinct()
            .OrderBy(x => x)
            .Skip(pagination.StartFrom)
            .Take(pagination.PerPage)
            .ToListAsync(cancellationToken);

        return epcs;
    }

    public async Task<IEnumerable<string>> ListDispositions(Pagination pagination, CancellationToken cancellationToken)
    {
        return await DistinctFromEvents(x => x.Disposition, pagination).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<string>> ListBizSteps(Pagination pagination, CancellationToken cancellationToken)
    {
        return await DistinctFromEvents(x => x.BusinessStep, pagination).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<string>> ListBizLocations(Pagination pagination, CancellationToken cancellationToken)
    {
        return await DistinctFromEvents(x => x.BusinessLocation, pagination).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<string>> ListReadPoints(Pagination pagination, CancellationToken cancellationToken)
    {
        return await DistinctFromEvents(x => x.ReadPoint, pagination).ToListAsync(cancellationToken);
    }

    private IQueryable<string> DistinctFromEvents(Expression<Func<Event, string>> selector, Pagination pagination)
    {
        return _context
            .QueryEvents(_user.DefaultQueryParameters)
            .Select(selector)
            .Where(x => x != null)
            .Distinct()
            .OrderBy(x => x)
            .Skip(pagination.StartFrom)
            .Take(pagination.PerPage);
    }
}
