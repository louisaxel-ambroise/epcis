using FasTnT.Application.Services.Users;
using FasTnT.Features.v2_0.Endpoints.Interfaces;
using FasTnT.Features.v2_0.Subscriptions;

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

        // WebSocket endpoints.
        app.MapHub<SubscriptionHub>("/v2_0/queries/{query}/subscriptions").AllowAnonymous();

        return app;
    }

    private static Task<IResult> HandleSubscriptionQuery(string query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private static Task<IResult> HandleSubscriptionDetailQuery(string query, string subscriptionId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private static Task<IResult> HandleDeleteSubscription(string query, string subscriptionId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private static Task<IResult> HandleSubscribeRequest(string query, SubscriptionRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

