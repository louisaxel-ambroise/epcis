using FasTnT.Application.UseCases.TopLevelResources;
using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;
using FasTnT.EfCore.Store;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FasTnT.Application.EfCore.UseCases.TopLevelResources;

public class TopLevelResourceUseCasesHandler :
    IListEpcsHandler,
    IListDispositionsHandler,
    IListEventTypesHandler,
    IListBizStepsHandler,
    IListBizLocationsHandler,
    IListReadPointsHandler
{
    private readonly EpcisContext _context;

    public TopLevelResourceUseCasesHandler(EpcisContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<string>> ListEpcs(Pagination pagination, CancellationToken cancellationToken)
    {
        var epcs = await _context.Set<Epc>()
            .AsNoTracking()
            .Select(x => x.Id)
            .Where(x => x != null)
            .Distinct()
            .OrderBy(x => x)
            .Skip(pagination.StartFrom)
            .Take(pagination.PerPage)
            .ToListAsync(cancellationToken);

        return epcs;
    }

    public Task<IEnumerable<string>> ListEventTypes(Pagination pagination, CancellationToken cancellationToken)
    {
        var eventTypes = Enum.GetValues<EventType>();

        return Task.FromResult(eventTypes.Select(x => x.ToString()));
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
        return _context.Events
               .AsNoTracking()
               .Select(selector)
               .Where(x => x != null)
               .Distinct()
               .OrderBy(x => x)
               .Skip(pagination.StartFrom)
               .Take(pagination.PerPage);
    }
}
