using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Features.v2_0.Endpoints.Interfaces;

public record ListSubscriptionsResult(IEnumerable<Subscription> Subscriptions);
