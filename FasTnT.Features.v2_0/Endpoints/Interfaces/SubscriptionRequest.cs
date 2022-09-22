using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Features.v2_0.Communication.Json.Parsers;

namespace FasTnT.Features.v2_0.Endpoints.Interfaces;

public record SubscriptionRequest(Subscription Subscription)
{
    public static async ValueTask<SubscriptionRequest> BindAsync(HttpContext context)
    {
        var subscription = await JsonRequestParser.ParseSubscriptionRequestAsync(context.Request.Body, context.RequestAborted);

        return new(subscription);
    }
}
