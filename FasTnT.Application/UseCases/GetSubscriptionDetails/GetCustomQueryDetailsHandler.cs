using FasTnT.Application.Store;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.UseCases.GetSubscriptionDetails
{
    internal class GetSubscriptionDetailsHandler : IGetSubscriptionDetailsHandler
    {
        private readonly EpcisContext _context;

        public GetSubscriptionDetailsHandler(EpcisContext context)
        {
            _context = context;
        }

        public async Task<Subscription> GetCustomQueryDetailsAsync(string name, CancellationToken cancellationToken)
        {
            var subscription = await _context.Subscriptions
                .AsNoTracking()
                .Where(x => x.Name == name)
                .Include(x => x.Parameters)
                .SingleOrDefaultAsync(x => x.Name == name, cancellationToken);

            if(subscription is null)
            {
                throw new EpcisException(ExceptionType.NoSuchNameException, $"Subscription not found: '{name}'");
            }

            return subscription;
        }
    }
}
