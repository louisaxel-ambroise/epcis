using FasTnT.Application.Services.Queries;
using FasTnT.Application.Store;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.UseCases.StoreStandardQuerySubscription
{
    public class StoreStandardQuerySubscriptionHandler : IStoreStandardQuerySubscriptionHandler
    {
        private readonly EpcisContext _context;
        private readonly IEnumerable<IStandardQuery> _queries;

        public StoreStandardQuerySubscriptionHandler(EpcisContext context, IEnumerable<IStandardQuery> queries)
        {
            _context = context;
            _queries = queries;
        }

        public async Task StoreSubscriptionAsync(Subscription subscription, CancellationToken cancellationToken)
        {
            var query = _queries.SingleOrDefault(x => x.Name == subscription.QueryName);

            if(query is null)
            {
                throw new EpcisException(ExceptionType.NoSuchNameException, $"Query with name '{subscription.QueryName}' does not exist");
            }
            if (!query.AllowSubscription)
            {
                throw new EpcisException(ExceptionType.SubscribeNotPermittedException, $"Subscription on query '{query.Name}' is not permitted");
            }
            if (await _context.Subscriptions.AnyAsync(x => x.Name == subscription.Name, cancellationToken))
            {
                throw new EpcisException(ExceptionType.DuplicateSubscriptionException, $"Subscription with name '{subscription.Name}' already exists");
            }

            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
