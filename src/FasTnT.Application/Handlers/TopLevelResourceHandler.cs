using FasTnT.Application.Database;
using FasTnT.Application.Services.Users;
using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FasTnT.Application.Handlers;

public class TopLevelResourceHandler(EpcisContext context, ICurrentUser user)
{
    public async Task<IEnumerable<string>> ListEpcs(Pagination pagination, CancellationToken cancellationToken)
    {
        var epcs = await context
            .QueryEvents(user.DefaultQueryParameters)
            .SelectMany(x => x.Epcs.Select(x => x.Id))
            .Distinct()
            .OrderBy(x => x)
            .Skip(pagination.StartFrom)
            .Take(pagination.PerPage)
            .ToListAsync(cancellationToken);

        return epcs;
    }

    public async Task<IEnumerable<EventType>> ListEventTypes(Pagination pagination, CancellationToken cancellationToken)
    {
        return await DistinctFromEvents(x => x.Type, pagination).ToListAsync(cancellationToken);
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

    private IQueryable<T> DistinctFromEvents<T>(Expression<Func<Event, T>> selector, Pagination pagination)
    {
        return context
            .QueryEvents(user.DefaultQueryParameters)
            .Select(selector)
            .Where(x => x != null)
            .Distinct()
            .OrderBy(x => x)
            .Skip(pagination.StartFrom)
            .Take(pagination.PerPage);
    }
}
