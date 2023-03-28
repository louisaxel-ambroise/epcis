using FasTnT.Host.Features.v1_2.Endpoints.Interfaces;
using FasTnT.Host.Features.v1_2.Extensions;
using FasTnT.Application.Handlers;
using FasTnT.Application.Domain.Exceptions;
using FasTnT.Application.Domain.Enumerations;
using FasTnT.Application;

namespace FasTnT.Host.Features.v1_2.Endpoints;

public static class SubscriptionEndpoints
{
    public static void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("v1_2/Trigger", TriggerSubscription).RequireAuthorization("query");
    }

    public static void AddSoapActions(SoapActionBuilder action)
    {
        action.On<GetSubscriptionIDs>(GetSubscriptionIds);
        action.On<Subscribe>(Subscribe);
        action.On<Unsubscribe>(Unsubscribe);
    }

    private static async Task<GetSubscriptionIDsResult> GetSubscriptionIds(GetSubscriptionIDs query, SubscriptionsHandler handler, CancellationToken cancellationToken)
    {
        var subscriptions = await handler.ListSubscriptionsAsync(query.QueryName, cancellationToken);

        return new GetSubscriptionIDsResult(subscriptions.Select(x => x.Name));
    }

    private static async Task<SubscribeResult> Subscribe(Subscribe request, SubscriptionsHandler handler, CancellationToken cancellationToken)
    {
        if(request.Subscription.QueryName != "SimpleEventQuery")
        {
            throw new EpcisException(ExceptionType.SubscribeNotPermittedException, $"Query does not allow subscription: {request.Subscription.QueryName}");
        }

        await handler.RegisterSubscriptionAsync(request.Subscription, cancellationToken);

        return new();
    }

    private static async Task<UnsubscribeResult> Unsubscribe(Unsubscribe request, SubscriptionsHandler handler, CancellationToken cancellationToken)
    {
        await handler.DeleteSubscriptionAsync(request.SubscriptionId, cancellationToken);

        return new();
    }

    private static IResult TriggerSubscription(string triggers)
    {
        if (string.IsNullOrWhiteSpace(triggers))
        {
            return Results.BadRequest();
        }

        EpcisEvents.SubscriptionTriggered(triggers.Split(';'));

        return Results.NoContent();
    }
}
