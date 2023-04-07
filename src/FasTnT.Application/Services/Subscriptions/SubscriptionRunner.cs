using FasTnT.Application.Database;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Events;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Services.Subscriptions;

public sealed class SubscriptionRunner
{
    private readonly EpcisContext _context;

    public SubscriptionRunner(EpcisContext context)
    {
        _context = context;
    }

    public async Task<SubscriptionResult> ExecuteAsync(SubscriptionContext executionContext, CancellationToken cancellationToken)
    {
        try
        {
            var pendingEvents = await _context
                .QueryEvents(executionContext.Parameters)
                .Select(x => new { x.Id, RequestId = x.Request.Id })
                .ToListAsync(cancellationToken);
            var eventIds = pendingEvents
                .Where(x => !executionContext.ExcludedRequestIds.Contains(x.RequestId))
                .Select(x => x.Id);
            var events = await _context.Set<Event>()
                .Where(x => eventIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

            return SubscriptionResult.Success(events, pendingEvents.Select(x => x.RequestId).Distinct().ToList());
        }
        catch (EpcisException ex)
        {
            return SubscriptionResult.Failed(ex);
        }
    }
}
