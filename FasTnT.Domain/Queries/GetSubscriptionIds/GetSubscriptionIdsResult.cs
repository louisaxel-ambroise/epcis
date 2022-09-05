namespace FasTnT.Domain.Queries;

public record GetSubscriptionIdsResult(IEnumerable<string> SubscriptionIDs) : IEpcisResponse;
