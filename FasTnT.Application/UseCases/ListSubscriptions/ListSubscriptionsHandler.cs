using FasTnT.Application.Store;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.UseCases.ListSubscriptions;

public class ListSubscriptionsHandler : IListSubscriptionsHandler
{
    private readonly EpcisContext _context;

    public ListSubscriptionsHandler(EpcisContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Subscription>> ListSubscriptionsAsync(string queryName, CancellationToken cancellationToken)
    {
        var subscriptions = await _context.Subscriptions
            .AsNoTracking()
            .Where(x => x.QueryName == queryName)
            .ToListAsync(cancellationToken);

        return subscriptions;
    }
}
