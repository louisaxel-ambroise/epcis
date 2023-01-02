using FasTnT.Application.UseCases.Subscriptions;
using FasTnT.Host.Features.v1_2.Subscriptions;
using FasTnT.Host.Features.v1_2.Endpoints.Interfaces;
using FasTnT.Host.Features.v1_2.Extensions;
using FasTnT.Domain.Exceptions;

namespace FasTnT.Host.Features.v1_2.Endpoints;

public static class SubscriptionEndpoints
{
    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("v1_2/Trigger", TriggerSubscription).RequireAuthorization("query");

        return app;
    }

    public static SoapActionBuilder AddSoapActions(SoapActionBuilder action)
    {
        action.On<GetSubscriptionIDs>(GetSubscriptionIds);
        action.On<Subscribe>(Subscribe);
        action.On<Unsubscribe>(Unsubscribe);

        return action;
    }

    private static async Task<GetSubscriptionIDsResult> GetSubscriptionIds(GetSubscriptionIDs query, IListSubscriptions handler, CancellationToken cancellationToken)
    {
        var subscriptions = await handler.ListSubscriptionsAsync(query.QueryName, cancellationToken);

        return new GetSubscriptionIDsResult(subscriptions.Select(x => x.Name));
    }

    private static async Task<SubscribeResult> Subscribe(Subscribe request, IRegisterSubscription handler, CancellationToken cancellationToken)
    {
        if(request.Subscription.QueryName != "SimpleEventQuery")
        {
            throw new EpcisException(ExceptionType.SubscribeNotPermittedException, $"Query does not allow subscription: {request.Subscription.QueryName}");
        }

        await handler.RegisterSubscriptionAsync(request.Subscription, XmlResultSender.Instance, cancellationToken);

        return new();
    }

    private static async Task<UnsubscribeResult> Unsubscribe(Unsubscribe request, IDeleteSubscription handler, CancellationToken cancellationToken)
    {
        await handler.DeleteSubscriptionAsync(request.SubscriptionId, cancellationToken);

        return new();
    }

    private static async Task<IResult> TriggerSubscription(string triggers, ITriggerSubscription handler, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(triggers))
        {
            return Results.BadRequest();
        }

        await handler.TriggerSubscriptionAsync(triggers.Split(';'), cancellationToken);

        return Results.NoContent();
    }
}
