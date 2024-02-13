using FasTnT.Application.Database;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Validators;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Handlers;

public class SubscriptionsHandler(EpcisContext context, ICurrentUser user)
{
    public async Task<Subscription> DeleteSubscriptionAsync(string name, CancellationToken cancellationToken)
    {
        var subscription = await context.Set<Subscription>().FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        if (subscription is null)
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Subscription '{name}' does not exist");
        }

        context.Remove(subscription);

        await context.SaveChangesAsync(cancellationToken);
        EpcisEvents.SubscriptionRemoved(subscription);

        return subscription;
    }

    public async Task<IEnumerable<Subscription>> ListSubscriptionsAsync(string queryName, CancellationToken cancellationToken)
    {
        var subscriptions = await context.Set<Subscription>()
            .Where(x => x.QueryName == queryName)
            .ToListAsync(cancellationToken);

        return subscriptions;
    }

    public async Task<Subscription> GetSubscriptionDetailsAsync(string name, CancellationToken cancellationToken)
    {
        var subscription = await context.Set<Subscription>()
            .Where(x => x.Name == name)
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        if (subscription is null)
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Subscription not found: '{name}'");
        }

        return subscription;
    }

    public async Task<Subscription> RegisterSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        if (!SubscriptionValidator.IsValid(subscription))
        {
            throw new EpcisException(ExceptionType.ValidationException, $"Subscription request is not valid");
        }
        if (await context.Set<Subscription>().AnyAsync(x => x.Name == subscription.Name, cancellationToken))
        {
            throw new EpcisException(ExceptionType.DuplicateSubscriptionException, $"Subscription '{subscription.Name}' already exists");
        }

        subscription.Parameters.AddRange(user.DefaultQueryParameters);

        context.Add(subscription);

        await context.SaveChangesAsync(cancellationToken);
        EpcisEvents.SubscriptionRegistered(subscription);

        return subscription;
    }
}
