using FasTnT.Application.Store;
using FasTnT.Application.Validators;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.UseCases.StoreCustomQuerySubscription
{
    public class StoreCustomQuerySubscriptionHandler : IStoreCustomQuerySubscriptionHandler
    {
        private readonly EpcisContext _context;

        public StoreCustomQuerySubscriptionHandler(EpcisContext context)
        {
            _context = context;
        }

        // TODO: notify subscription listeners that a new subscription was added.
        public async Task<Subscription> StoreSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken)
        {
            if (!SubscriptionValidator.IsValid(subscription))
            {
                throw new EpcisException(ExceptionType.ValidationException, "Subscription is not valid");
            }
            if(await _context.Subscriptions.AnyAsync(x => x.Name == subscription.Name))
            {
                throw new EpcisException(ExceptionType.DuplicateSubscriptionException, $"Subscription with name '{subscription.Name}' already exists");
            }

            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync(cancellationToken);

            return subscription;
        }
    }
}
