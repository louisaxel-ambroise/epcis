using FasTnT.Domain.Queries.Poll;

namespace FasTnT.Domain.Queries.GetSubscriptionIds;

public record GetSubscriptionIdsResult(IEnumerable<string> SubscriptionIDs) : IEpcisResponse;
