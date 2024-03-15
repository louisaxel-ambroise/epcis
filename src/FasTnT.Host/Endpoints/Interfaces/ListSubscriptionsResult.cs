using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Host.Endpoints.Interfaces;

public record ListSubscriptionsResult(IEnumerable<Subscription> Subscriptions);

public record ListSubscriptionsRequest(string QueryName);