using FasTnT.Application.Domain.Model.Subscriptions;

namespace FasTnT.Host.Features.v2_0.Endpoints.Interfaces;

public record ListSubscriptionsResult(IEnumerable<Subscription> Subscriptions);
