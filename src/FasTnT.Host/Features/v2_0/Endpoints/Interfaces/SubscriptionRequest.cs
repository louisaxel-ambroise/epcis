using FasTnT.Application.Domain.Model.Subscriptions;
using FasTnT.Host.Features.v2_0.Communication.Json.Parsers;

namespace FasTnT.Host.Features.v2_0.Endpoints.Interfaces;

public record SubscriptionRequest(string QueryName, Subscription Subscription)
{
    public static async ValueTask<SubscriptionRequest> BindAsync(HttpContext context)
    {
        var subscription = await JsonRequestParser.ParseSubscriptionRequestAsync(context.Request.Body, context.RequestAborted);
        var queryName = context.Request.RouteValues.GetValueOrDefault("query")?.ToString();

        return new(queryName, subscription);
    }
}
