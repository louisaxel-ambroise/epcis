using FasTnT.Application.Services.Users;
using FasTnT.Application.UseCases.ListSubscriptions;
using FasTnT.Application.UseCases.StoreCustomQuerySubscription;
using FasTnT.Features.v2_0.Endpoints.Interfaces;

namespace FasTnT.Features.v2_0.Endpoints;

public class SubscriptionEndpoints
{
    protected SubscriptionEndpoints() { }

    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/v2_0/queries/{query}/subscriptions", HandleSubscriptionQuery).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));
        app.MapGet("/v2_0/queries/{query}/subscriptions/{subscriptionId}", HandleSubscriptionDetailQuery).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery)).WithName("SubscriptionDetail");
        app.MapDelete("/v2_0/queries/{query}/subscriptions/{subscriptionId}", HandleDeleteSubscription).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));
        app.MapPost("/v2_0/queries/{query}/subscriptions", HandleSubscribeRequest).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));

        // TODO: add WebSocket endpoints.

        return app;
    }

    private static async Task<IResult> HandleSubscriptionQuery(string query, IListSubscriptionsHandler handler, CancellationToken cancellationToken)
    {
        var subscriptions = await handler.ListSubscriptionsAsync(query, cancellationToken);

        return EpcisResults.Ok(subscriptions);
    }

    private static Task<IResult> HandleSubscriptionDetailQuery(string query, string subscriptionId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private static Task<IResult> HandleDeleteSubscription(string query, string subscriptionId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private static Task<IResult> HandleSubscribeRequest(string query, SubscriptionRequest request, IStoreCustomQuerySubscriptionHandler handler, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

