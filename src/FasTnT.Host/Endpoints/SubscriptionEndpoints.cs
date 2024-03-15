using FasTnT.Application.Handlers;
using FasTnT.Domain.Exceptions;
using FasTnT.Host.Endpoints.Interfaces;
using FasTnT.Host.Endpoints.Responses.Rest;
using FasTnT.Host.Extensions;

namespace FasTnT.Host.Endpoints;

public static class SubscriptionEndpoints
{
    public static IEndpointRouteBuilder AddSubscriptionEndpoints(this IEndpointRouteBuilder app)
    {
        app.Get("queries/{query}/subscriptions", SubscriptionQuery).RequireAuthorization("query");
        app.Get("queries/{query}/subscriptions/{name}", SubscriptionDetailQuery).RequireAuthorization("query");
        app.Delete("queries/{query}/subscriptions/{name}", DeleteSubscription).RequireAuthorization("query");
        app.Post("queries/{query}/subscriptions", SubscribeRequest).RequireAuthorization("query");

        return app;
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

        var response = await subscribe.RegisterSubscriptionAsync(request.Subscription, cancellationToken);

        return Results.Created($"queries/{query}/subscriptions/{response.Name}", null);
    }
}
