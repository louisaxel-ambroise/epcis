using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Services.Subscriptions;

public record SubscriptionContext(IEnumerable<QueryParameter> Parameters, IEnumerable<int> ExcludedRequestIds);