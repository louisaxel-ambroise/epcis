using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Host.Communication.Json.Parsers;

namespace FasTnT.Host.Endpoints.Interfaces;

public record SubscriptionRequest(string QueryName, Subscription Subscription)
{
    public static async ValueTask<SubscriptionRequest> BindAsync(HttpContext context)
    {
        var subscription = await JsonRequestParser.ParseSubscriptionRequestAsync(context.Request.Body, context.RequestAborted);
        var queryName = context.Request.RouteValues.GetValueOrDefault("query")?.ToString();

        return new(queryName, subscription);
    }
}
