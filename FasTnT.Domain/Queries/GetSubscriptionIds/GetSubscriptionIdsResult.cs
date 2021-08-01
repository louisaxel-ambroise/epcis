using System.Collections.Generic;

namespace FasTnT.Domain.Queries.GetSubscriptionIds
{
    public record GetSubscriptionIdsResult(IEnumerable<string> SubscriptionIDs);
}