using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Features.v2_0.Endpoints.Interfaces;

public record SubscriptionRequest(Subscription Subscription)
{
    public static ValueTask<SubscriptionRequest> BindAsync(HttpContext context)
    {
        throw new NotImplementedException();
    }
}
