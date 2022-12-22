﻿using FasTnT.Application.Database;
using FasTnT.Application.Services.Subscriptions;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Validators;
using FasTnT.Domain.Exceptions;
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
    private readonly ICurrentUser _currentUser;
    private readonly ISubscriptionListener _listener;

    public SubscriptionsUseCasesHandler(EpcisContext context, ICurrentUser currentUser, ISubscriptionListener listener)
    {
        _context = context;
        _currentUser = currentUser;
        _listener = listener;
    }

    public async Task<Subscription> DeleteSubscriptionAsync(string name, CancellationToken cancellationToken)
    {
        var subscription = await _context.Set<Subscription>().FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

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
        await _listener.RegisterAsync(new (subscription, resultSender), cancellationToken);

        return subscription;
    }
}
