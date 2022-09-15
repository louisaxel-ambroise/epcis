using FasTnT.Application.Services.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using FasTnT.Features.v1_2.Extensions;
using FasTnT.Features.v1_2.Endpoints.Interfaces;
using FasTnT.Application.UseCases.Subscriptions;

namespace FasTnT.Features.v1_2.Endpoints;

public class SubscriptionEndpoints
{
    protected SubscriptionEndpoints() { }

    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("v1_2/Trigger", HandleTriggerSubscription).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));

        return app;
    }

    public static SoapActionBuilder AddSoapActions(SoapActionBuilder action)
    {
        action.On<GetSubscriptionIDs>(HandleGetSubscriptionIds);
        action.On<Subscribe>(HandleSubscribe);
        action.On<Unsubscribe>(HandleUnsubscribe);

        return action;
    }

    private static async Task<GetSubscriptionIDsResult> HandleGetSubscriptionIds(GetSubscriptionIDs query, IListSubscriptionsHandler handler, CancellationToken cancellationToken)
    {
        var subscriptions = await handler.ListSubscriptionsAsync(query.QueryName, cancellationToken);

        return new GetSubscriptionIDsResult(subscriptions.Select(x => x.Name));
    }

    private static async Task<SubscribeResult> HandleSubscribe(Subscribe request, IStandardQuerySubscriptionHandler handler, CancellationToken cancellationToken)
    {
        await handler.StandardQuerySubscriptionAsync(request.Subscription, cancellationToken);

        return new();
    }

    private static async Task<UnsubscribeResult> HandleUnsubscribe(Unsubscribe request, IDeleteSubscriptionHandler handler, CancellationToken cancellationToken)
    {
        await handler.DeleteSubscriptionAsync(request.SubscriptionId, cancellationToken);

        return new();
    }

    private static async Task<IResult> HandleTriggerSubscription(string triggers, ITriggerSubscriptionHandler handler, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(triggers))
        {
            return Results.BadRequest();
        }

        await handler.TriggerSubscriptionAsync(triggers.Split(';'), cancellationToken);

        return Results.NoContent();
    }
}
