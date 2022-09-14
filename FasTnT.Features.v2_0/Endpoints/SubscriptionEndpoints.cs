using FasTnT.Application.Services.Users;
using FasTnT.Application.UseCases.DeleteSubscription;
using FasTnT.Application.UseCases.GetSubscriptionDetails;
using FasTnT.Application.UseCases.ListSubscriptions;
using FasTnT.Application.UseCases.StoreCustomQuerySubscription;
using FasTnT.Features.v2_0.Endpoints.Interfaces;
using FasTnT.Features.v2_0.Endpoints.Interfaces.Utils;

namespace FasTnT.Features.v2_0.Endpoints;

public class SubscriptionEndpoints
{
    protected SubscriptionEndpoints() { }

    // TODO: add WebSocket endpoints.
    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/v2_0/queries/{query}/subscriptions", HandleSubscriptionQuery).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));
        app.MapGet("/v2_0/queries/{query}/subscriptions/{name}", HandleSubscriptionDetailQuery).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery)).WithName("SubscriptionDetail");
        app.MapDelete("/v2_0/queries/{query}/subscriptions/{name}", HandleDeleteSubscription).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));
        app.MapPost("/v2_0/queries/{query}/subscriptions", HandleSubscribeRequest).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));

        return app;
    }

    private static async Task<IResult> HandleSubscriptionQuery(string query, IListSubscriptionsHandler handler, CancellationToken cancellationToken)
    {
        var subscriptions = await handler.ListSubscriptionsAsync(query, cancellationToken);

        return EpcisResults.Ok(subscriptions);
    }

    private static async Task<IResult> HandleSubscriptionDetailQuery(string name, IGetSubscriptionDetailsHandler handler, CancellationToken cancellationToken)
    {
        var subscription = await handler.GetCustomQueryDetailsAsync(name, cancellationToken);

        return EpcisResults.Ok(subscription);
    }

    private static async Task<IResult> HandleDeleteSubscription(string name, IDeleteSubscriptionHandler handler, CancellationToken cancellationToken)
    {
        await handler.DeleteSubscriptionAsync(name, cancellationToken);

        return Results.NoContent();
    }

    private static async Task<IResult> HandleSubscribeRequest(string query, SubscriptionRequest request, IStoreCustomQuerySubscriptionHandler handler, CancellationToken cancellationToken)
    {
        request.Subscription.QueryName = query;

        await handler.StoreSubscriptionAsync(request.Subscription, cancellationToken);

        return Results.Created($"v2_0/queries/{query}/subscriptions/{request.Subscription.Name}", null);
    }
}

