using FasTnT.Application.Store;
using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model.Events;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.UseCases.ListTopLevelResources;

internal class ListTopResourcesHandler : 
    IListEpcsHandler,
    IListDispositionsHandler,
    IListEventTypesHandler,
    IListBizStepsHandler,
    IListBizLocationsHandler,
    IListReadPointsHandler
{
    private readonly EpcisContext _context;

    public ListTopResourcesHandler(EpcisContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<string>> ListEpcs(CancellationToken cancellationToken)
    {
        var epcs = await _context.Set<Epc>()
            .AsNoTracking()
            .Select(x => x.Id)
            .Distinct()
            .ToListAsync(cancellationToken);

        return epcs;
    }

    public async Task<IEnumerable<string>> ListDispositions(CancellationToken cancellationToken)
    {
        var dispositions = await _context.Events
            .AsNoTracking()
            .Select(x => x.Disposition)
            .Distinct()
            .ToListAsync(cancellationToken);

        return dispositions;
    }

    public Task<IEnumerable<EventType>> ListEventTypes(CancellationToken cancellationToken)
    {
        var eventTypes = Enum.GetValues<EventType>();

        return Task.FromResult(eventTypes.AsEnumerable());
    }

    public async Task<IEnumerable<string>> ListBizSteps(CancellationToken cancellationToken)
    {
        var bizSteps = await _context.Events
            .AsNoTracking()
            .Select(x => x.BusinessStep)
            .Distinct()
            .ToListAsync(cancellationToken);

        return bizSteps;
    }

    public async Task<IEnumerable<string>> ListBizLocations(CancellationToken cancellationToken)
    {
        var bizLocations = await _context.Events
            .AsNoTracking()
            .Select(x => x.BusinessLocation)
            .Distinct()
            .ToListAsync(cancellationToken);

        return bizLocations;
    }

    public async Task<IEnumerable<string>> ListReadPoints(CancellationToken cancellationToken)
    {
        var readPoints = await _context.Events
            .AsNoTracking()
            .Select(x => x.ReadPoint)
            .Distinct()
            .ToListAsync(cancellationToken);

        return readPoints;
    }
}
