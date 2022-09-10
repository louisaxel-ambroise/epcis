using FasTnT.Application.Store;
using FasTnT.Domain.Queries.GetSubscriptionIds;
using FasTnT.Domain.Queries.Poll;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Queries.GetSubscriptionIds;

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
