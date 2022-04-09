using FasTnT.Domain.Commands.Unsubscribe;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Notifications;
using FasTnT.Infrastructure.Store;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Subscriptions;

public class UnsubscribeCommandHandler : IRequestHandler<UnsubscribeCommand, UnsubscribeResult>
{
    private readonly EpcisContext _context;
    private readonly IMediator _mediator;

    public UnsubscribeCommandHandler(EpcisContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<UnsubscribeResult> Handle(UnsubscribeCommand request, CancellationToken cancellationToken)
    {
        var subscription = await _context.Subscriptions.SingleOrDefaultAsync(x => x.Name == request.SubscriptionId, cancellationToken);

        if(subscription == default)
        {
            throw new EpcisException(ExceptionType.NoSuchSubscriptionException, "Subscription does not exist");
        }

        await _mediator.Publish(new SubscriptionRemovedNotification(subscription.Id), cancellationToken);

        _context.Subscriptions.Remove(subscription);

        await _context.SaveChangesAsync(cancellationToken);

        return new();
    }
}
