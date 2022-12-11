using FasTnT.Application.UseCases.Subscriptions;
using FasTnT.Host.Features.v1_2.Subscriptions;
using FasTnT.Host.Features.v1_2.Endpoints.Interfaces;
using FasTnT.Host.Features.v1_2.Extensions;

namespace FasTnT.Host.Features.v1_2.Endpoints;

public static class SubscriptionEndpoints
{
    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("v1_2/Trigger", HandleTriggerSubscription).RequireAuthorization("query");

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

    private static async Task<SubscribeResult> HandleSubscribe(Subscribe request, IRegisterSubscriptionHandler handler, CancellationToken cancellationToken)
    {
        await handler.RegisterSubscriptionAsync(request.Subscription, XmlResultSender.Instance, cancellationToken);

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
