using FasTnT.Application.EfCore.Store;
using FasTnT.Application.UseCases.TopLevelResources;
using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model.Events;
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

    public async Task<IEnumerable<string>> ListEpcs(CancellationToken cancellationToken)
    {
        var epcs = await _context.Set<Epc>()
            .AsNoTracking()
            .Select(x => x.Id)
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct()
            .ToListAsync(cancellationToken);

        return epcs;
    }

    public Task<IEnumerable<string>> ListEventTypes(CancellationToken cancellationToken)
    {
        var eventTypes = Enum.GetValues<EventType>();

        return Task.FromResult(eventTypes.Select(x => x.ToString()));
    }

    public Task<IEnumerable<string>> ListDispositions(CancellationToken cancellationToken)
    {
        return DistinctFromEvents(x => x.Disposition, cancellationToken);
    }

    public Task<IEnumerable<string>> ListBizSteps(CancellationToken cancellationToken)
    {
        return DistinctFromEvents(x => x.BusinessStep, cancellationToken);
    }

    public Task<IEnumerable<string>> ListBizLocations(CancellationToken cancellationToken)
    {
        return DistinctFromEvents(x => x.BusinessLocation, cancellationToken);
    }

    public Task<IEnumerable<string>> ListReadPoints(CancellationToken cancellationToken)
    {
        return DistinctFromEvents(x => x.ReadPoint, cancellationToken);
    }

    private async Task<IEnumerable<string>> DistinctFromEvents(Expression<Func<Event, string>> selector, CancellationToken cancellationToken)
    {
        return await _context.Events
               .AsNoTracking()
               .Select(selector)
               .Where(x => !string.IsNullOrEmpty(x))
               .Distinct()
               .ToListAsync(cancellationToken);
    }
}
