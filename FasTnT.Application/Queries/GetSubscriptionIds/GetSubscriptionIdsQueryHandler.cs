using FasTnT.Domain.Queries;
using FasTnT.Infrastructure.Store;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Queries;
 
public class GetSubscriptionIdsQueryHandler : IRequestHandler<GetSubscriptionIdsQuery, IEpcisResponse>
{
    private readonly EpcisContext _context;

    public GetSubscriptionIdsQueryHandler(EpcisContext context)
    {
        _context = context;
    }

    public async Task<IEpcisResponse> Handle(GetSubscriptionIdsQuery request, CancellationToken cancellationToken)
    {
        var subscriptionIds = await _context.Subscriptions
            .AsNoTracking()
            .Where(x => x.QueryName == request.QueryName)
            .Select(x => x.Name)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        return new GetSubscriptionIdsResult(subscriptionIds);
    }
}
