using FasTnT.Application.Handlers;
using FasTnT.Domain.Model.CustomQueries;
using FasTnT.Host.Features.v2_0.Endpoints.Interfaces;
using FasTnT.Host.Features.v2_0.Endpoints.Interfaces.Utils;
using FasTnT.Host.Features.v2_0.Subscriptions;

namespace FasTnT.Host.Features.v2_0.Endpoints;

public static class SubscriptionEndpoints
{
    public static void AddRoutes(IEndpointRouteBuilder app)
    {
        app.Get("v2_0/queries/{query}/subscriptions", SubscriptionQuery).RequireAuthorization("query");
        app.Get("v2_0/queries/{query}/subscriptions/{name}", SubscriptionDetailQuery).RequireAuthorization("query").WithName("SubscriptionDetail");
        app.MapDelete("v2_0/queries/{query}/subscriptions/{name}", DeleteSubscription).RequireAuthorization("query");
        app.Post("v2_0/queries/{query}/subscriptions", SubscribeRequest).RequireAuthorization("query");
    }

    private static async Task<IResult> SubscriptionQuery(string query, SubscriptionsHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListSubscriptionsAsync(query, cancellationToken);

        return EpcisResults.Ok(new ListSubscriptionsResult(response));
    }

    private static async Task<IResult> SubscriptionDetailQuery(string name, SubscriptionsHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.GetSubscriptionDetailsAsync(name, cancellationToken);

        return EpcisResults.Ok(new SubscriptionDetailsResult(response));
    }

    private static async Task<IResult> DeleteSubscription(string name, SubscriptionsHandler handler, CancellationToken cancellationToken)
    {
        await handler.DeleteSubscriptionAsync(name, cancellationToken);

        return Results.NoContent();
    }

    private static async Task<IResult> SubscribeRequest(SubscriptionRequest request, QueriesHandler queryDetails, SubscriptionsHandler subscribe, CancellationToken cancellationToken)
    {
        var query = await queryDetails.GetQueryDetailsAsync(request.QueryName, cancellationToken);

        request.Subscription.QueryName = query.Name;
        request.Subscription.Name = Guid.NewGuid().ToString();
        request.Subscription.Parameters.AddRange(query.Parameters);

        var response = await subscribe.RegisterSubscriptionAsync(request.Subscription, JsonResultSender.Instance, cancellationToken);

        return Results.Created($"v2_0/queries/{query}/subscriptions/{response.Name}", null);
    }
}
