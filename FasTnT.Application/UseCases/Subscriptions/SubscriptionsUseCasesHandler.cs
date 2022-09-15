using FasTnT.Application.Services.Queries;
using FasTnT.Application.Services.Subscriptions;
using FasTnT.Application.Store;
using FasTnT.Application.Validators;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.UseCases.Subscriptions;

public class SubscriptionsUseCasesHandler :
    IDeleteSubscriptionHandler,
    IListSubscriptionsHandler,
    IGetSubscriptionDetailsHandler,
    ITriggerSubscriptionHandler,
    IStandardQuerySubscriptionHandler,
    ICustomQuerySubscriptionHandler
{
    private readonly EpcisContext _context;
    private readonly IEnumerable<IStandardQuery> _queries;
    private readonly IEnumerable<ISubscriptionListener> _listeners;

    public SubscriptionsUseCasesHandler(EpcisContext context, IEnumerable<IStandardQuery> queries, IEnumerable<ISubscriptionListener> listeners)
    {
        _context = context;
        _queries = queries;
        _listeners = listeners;
    }

    public async Task<Subscription> DeleteSubscriptionAsync(string name, CancellationToken cancellationToken)
    {
        var subscription = await _context.Subscriptions.SingleOrDefaultAsync(x => x.Name == name, cancellationToken);

        if (subscription is null)
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Subscription '{name}' does not exist");
        }

        _context.Subscriptions.Remove(subscription);
        await _context.SaveChangesAsync(cancellationToken);

        return subscription;
    }

    public async Task<IEnumerable<Subscription>> ListSubscriptionsAsync(string queryName, CancellationToken cancellationToken)
    {
        var subscriptions = await _context.Subscriptions
            .AsNoTracking()
            .Where(x => x.QueryName == queryName)
            .ToListAsync(cancellationToken);

        return subscriptions;
    }

    public async Task<Subscription> GetSubscriptionDetailsAsync(string name, CancellationToken cancellationToken)
    {
        var subscription = await _context.Subscriptions
            .AsNoTracking()
            .Where(x => x.Name == name)
            .Include(x => x.Parameters)
            .SingleOrDefaultAsync(x => x.Name == name, cancellationToken);

        if (subscription is null)
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Subscription not found: '{name}'");
        }

        return subscription;
    }

    public async Task TriggerSubscriptionAsync(string[] triggers, CancellationToken cancellationToken)
    {
        await Task.WhenAll(_listeners.Select(x => x.TriggerAsync(triggers, cancellationToken)));
    }

    public async Task<Subscription> StandardQuerySubscriptionAsync(StandardSubscription subscription, CancellationToken cancellationToken)
    {
        var query = _queries.SingleOrDefault(x => x.Name == subscription.QueryName);

        if (!SubscriptionValidator.IsValid(subscription))
        {
            throw new EpcisException(ExceptionType.ValidationException, $"Subscription request is not valid");
        }
        if (query is null) 
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Query with name '{subscription.QueryName}' not found");
        }
        if(!query.AllowSubscription) 
        {
            throw new EpcisException(ExceptionType.SubscribeNotPermittedException, $"Query '{subscription.QueryName}' does not allow subscription");
        }
        if(await _context.Subscriptions.AnyAsync(x => x.Name == subscription.Name, cancellationToken)) 
        {
            throw new EpcisException(ExceptionType.DuplicateSubscriptionException, $"Subscription '{subscription.Name}' already exists");
        }

        return await RegisterSubscriptionAsync(subscription, cancellationToken);
    }

    public async Task<Subscription> CustomQuerySubscriptionAsync(CustomSubscription subscription, CancellationToken cancellationToken)
    {
        var query = await _context.CustomQueries
            .AsNoTracking()
            .Include(x => x.Parameters)
            .SingleOrDefaultAsync(x => x.Name == subscription.QueryName, cancellationToken);

        if (!SubscriptionValidator.IsValid(subscription))
        {
            throw new EpcisException(ExceptionType.ValidationException, $"Subscription request is not valid");
        }
        if (query is null)
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Query with name '{subscription.QueryName}' not found");
        }
        if (await _context.Subscriptions.AnyAsync(x => x.Name == subscription.Name, cancellationToken))
        {
            throw new EpcisException(ExceptionType.DuplicateSubscriptionException, $"Subscription '{subscription.Name}' already exists");
        }

        return await RegisterSubscriptionAsync(subscription, cancellationToken);
    }

    private async Task<Subscription> RegisterSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        _context.Subscriptions.Add(subscription);

        await _context.SaveChangesAsync(cancellationToken);
        await Task.WhenAll(_listeners.Select(x => x.RegisterAsync(subscription, cancellationToken)));

        return subscription;
    }
}
