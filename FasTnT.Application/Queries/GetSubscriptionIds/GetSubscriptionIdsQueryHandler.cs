using FasTnT.Domain.Queries.GetSubscriptionIds;
using FasTnT.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Application.Queries.GetQueryNames
{
    public class GetSubscriptionIdsQueryHandler : IRequestHandler<GetSubscriptionIdsQuery, GetSubscriptionIdsResult>
    {
        private readonly EpcisContext _context;

        public GetSubscriptionIdsQueryHandler(EpcisContext context)
        {
            _context = context;
        }

        public async Task<GetSubscriptionIdsResult> Handle(GetSubscriptionIdsQuery request, CancellationToken cancellationToken)
        {
            var subscriptions = await _context.Subscriptions.Select(x => x.Name).ToListAsync(cancellationToken);

            return new(subscriptions);
        }
    }
}
