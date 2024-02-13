using FasTnT.Application.Database;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Events;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Services.Subscriptions;

public sealed class SubscriptionRunner(EpcisContext context)
{
    public async Task<SubscriptionResult> ExecuteAsync(SubscriptionContext executionContext, CancellationToken cancellationToken)
    {
        try
        {
            var pendingEvents = await context
                .QueryEvents(executionContext.Parameters)
                .Select(x => new { x.Id, RequestId = x.Request.Id })
                .OrderBy(x => x.Id)
                .ToListAsync(cancellationToken);
            var eventIds = pendingEvents
                .ExceptBy(executionContext.ExcludedRequestIds, x => x.RequestId)
                .Select(x => x.Id);
            var events = await context.Set<Event>()
                .Where(x => eventIds.Contains(x.Id))
                .OrderBy(x => x.Id)
                .ToListAsync(cancellationToken);

            return SubscriptionResult.Success(events, pendingEvents.Select(x => x.RequestId).Distinct().ToList());
        }
        catch (EpcisException ex)
        {
            return SubscriptionResult.Failed(ex);
        }
    }
}
