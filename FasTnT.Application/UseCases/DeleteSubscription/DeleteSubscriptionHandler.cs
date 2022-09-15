using FasTnT.Application.Store;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.UseCases.DeleteSubscription;

public class DeleteSubscriptionHandler : IDeleteSubscriptionHandler
{
    private readonly EpcisContext _context;

    public DeleteSubscriptionHandler(EpcisContext context)
    {
        _context = context;
    }

    public async Task<Subscription> DeleteSubscriptionAsync(string name, CancellationToken cancellationToken)
    {
        var subscription = await _context.Subscriptions.SingleOrDefaultAsync(x => x.Name == name, cancellationToken);

        if(subscription is null)
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Subscription '{name}' does not exist");
        }

        _context.Subscriptions.Remove(subscription);
        await _context.SaveChangesAsync(cancellationToken);

        return subscription;
    }
}
