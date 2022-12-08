using FasTnT.Application.Services.Queries;
using FasTnT.Application.Services.Subscriptions;
using FasTnT.Application.Validators;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.CustomQueries;
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
    private readonly IEnumerable<IEpcisDataSource> _queries;
    private readonly ISubscriptionListener _listener;

    public SubscriptionsUseCasesHandler(EpcisContext context, IEnumerable<IEpcisDataSource> queries, ISubscriptionListener listener)
    {
        _context = context;
        _queries = queries;
        _listener = listener;
    }

    public async Task<Subscription> DeleteSubscriptionAsync(string name, CancellationToken cancellationToken)
    {
        var subscription = await _context.Set<Subscription>().SingleOrDefaultAsync(x => x.Name == name, cancellationToken);

        if (subscription is null)
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Subscription '{name}' does not exist");
        }

        _context.Remove(subscription);
        await _context.SaveChangesAsync(cancellationToken);
        await _listener.RemoveAsync(subscription.Id, cancellationToken);

        return subscription;
    }

    public async Task<IEnumerable<Subscription>> ListSubscriptionsAsync(string queryName, CancellationToken cancellationToken)
    {
        var subscriptions = await _context.Set<Subscription>()
            .AsNoTracking()
            .Where(x => x.QueryName == queryName)
            .ToListAsync(cancellationToken);

        return subscriptions;
    }

    public async Task<Subscription> GetSubscriptionDetailsAsync(string name, CancellationToken cancellationToken)
    {
        var subscription = await _context.Set<Subscription>()
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

    public async Task<Subscription> RegisterSubscriptionAsync(Subscription subscription, IResultSender resultSender, CancellationToken cancellationToken)
    {
        var query = await _context.Set<StoredQuery>()
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

        var dataSource = _queries.SingleOrDefault(x => x.Name == query.DataSource);

        if (resultSender is null)
        {
            throw new EpcisException(ExceptionType.ImplementationException, $"Subscription result formatter '{subscription.FormatterName}' not found");
        }
        if (dataSource is null)
        {
            throw new EpcisException(ExceptionType.SubscribeNotPermittedException, $"Query '{subscription.QueryName}' has an invalid dataSource");
        }
        if (!dataSource.AllowSubscription)
        {
            throw new EpcisException(ExceptionType.SubscribeNotPermittedException, $"Query '{subscription.QueryName}' does not allow subscription");
        }
        if (await _context.Set<Subscription>().CountAsync(x => x.Name == subscription.Name, cancellationToken) > 0)
        {
            throw new EpcisException(ExceptionType.DuplicateSubscriptionException, $"Subscription '{subscription.Name}' already exists");
        }

        subscription.Query = query;
        subscription.Parameters.AddRange(query.Parameters.Select(x => new SubscriptionParameter
        {
            Name = x.Name,
            Values = x.Values
        }));

        _context.Add(subscription);

        await _context.SaveChangesAsync(cancellationToken);
        await _listener.RegisterAsync(new SubscriptionContext(subscription, resultSender), cancellationToken);

        return subscription;
    }
}
