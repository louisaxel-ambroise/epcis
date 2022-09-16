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
    IRegisterSubscriptionHandler
{
    private readonly EpcisContext _context;
    private readonly IEnumerable<IEpcisDataSource> _dataSources;
    private readonly ISubscriptionListener _listener;

    public SubscriptionsUseCasesHandler(EpcisContext context, IEnumerable<IEpcisDataSource> queries, ISubscriptionListener listener)
    {
        _context = context;
        _dataSources = queries;
        _listener = listener;
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
        await _listener.RemoveAsync(subscription, cancellationToken);

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
        await _listener.TriggerAsync(triggers, cancellationToken);
    }

    public async Task<Subscription> RegisterSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        var dataSource = _dataSources.SingleOrDefault(x => x.Name == subscription.QueryName);

        if (!SubscriptionValidator.IsValid(subscription))
        {
            throw new EpcisException(ExceptionType.ValidationException, $"Subscription request is not valid");
        }
        if (dataSource is null) 
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Query with name '{subscription.QueryName}' not found");
        }
        if(!dataSource.AllowSubscription) 
        {
            throw new EpcisException(ExceptionType.SubscribeNotPermittedException, $"Query '{subscription.QueryName}' does not allow subscription");
        }
        if(await _context.Subscriptions.AnyAsync(x => x.Name == subscription.Name, cancellationToken)) 
        {
            throw new EpcisException(ExceptionType.DuplicateSubscriptionException, $"Subscription '{subscription.Name}' already exists");
        }

        _context.Subscriptions.Add(subscription);

        await _context.SaveChangesAsync(cancellationToken);
        await _listener.RegisterAsync(subscription, cancellationToken);

        return subscription;
    }
}
