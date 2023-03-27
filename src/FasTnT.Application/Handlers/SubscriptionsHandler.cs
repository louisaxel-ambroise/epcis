using FasTnT.Application.Domain.Exceptions;
using FasTnT.Application.Domain.Model.Subscriptions;
using FasTnT.Application.Domain.Validators;
using FasTnT.Application.Services.Storage;
using FasTnT.Application.Services.Subscriptions;
using FasTnT.Application.Services.Users;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Handlers;

public class SubscriptionsHandler
{
    private readonly EpcisContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly ISubscriptionListener _listener;

    public SubscriptionsHandler(EpcisContext context, ICurrentUser currentUser, ISubscriptionListener listener)
    {
        _context = context;
        _currentUser = currentUser;
        _listener = listener;
    }

    public async Task<Subscription> DeleteSubscriptionAsync(string name, CancellationToken cancellationToken)
    {
        var subscription = await _context.Set<Subscription>()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        if (subscription is null)
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Subscription '{name}' does not exist");
        }

        _context.Remove(subscription);
        await _context.SaveChangesAsync(cancellationToken);
        await _listener.RemoveAsync(subscription.Name, cancellationToken);

        return subscription;
    }

    public async Task<IEnumerable<Subscription>> ListSubscriptionsAsync(string queryName, CancellationToken cancellationToken)
    {
        var subscriptions = await _context.Set<Subscription>()
            .Where(x => x.QueryName == queryName)
            .ToListAsync(cancellationToken);

        return subscriptions;
    }

    public async Task<Subscription> GetSubscriptionDetailsAsync(string name, CancellationToken cancellationToken)
    {
        var subscription = await _context.Set<Subscription>()
            .Where(x => x.Name == name)
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        if (subscription is null)
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Subscription not found: '{name}'");
        }

        return subscription;
    }

    public async Task TriggerSubscriptionAsync(string[] triggers, CancellationToken cancellationToken)
    {
        await _listener.TriggerAsync(triggers, cancellationToken);
    }

    public async Task<Subscription> RegisterSubscriptionAsync(Subscription subscription, IResultSender resultSender, CancellationToken cancellationToken)
    {
        if (!SubscriptionValidator.IsValid(subscription))
        {
            throw new EpcisException(ExceptionType.ValidationException, $"Subscription request is not valid");
        }
        if (await _context.Set<Subscription>().AnyAsync(x => x.Name == subscription.Name, cancellationToken))
        {
            throw new EpcisException(ExceptionType.DuplicateSubscriptionException, $"Subscription '{subscription.Name}' already exists");
        }

        subscription.Parameters.AddRange(_currentUser.DefaultQueryParameters);

        _context.Add(subscription);

        await _context.SaveChangesAsync(cancellationToken);
        await _listener.RegisterAsync(new(subscription, resultSender), cancellationToken);

        return subscription;
    }
}
