using FasTnT.Application.UseCases.Subscriptions;
using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.Extensions.DependencyInjection;
using System.Net.WebSockets;
using System.Web;

namespace FasTnT.Features.v2_0.Subscriptions;

public static class WebSocketSubscription
{
    public async static Task SubscribeAsync(HttpContext httpContext, string queryName, IEnumerable<QueryParameter> parameters)
    {
        using var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();

        var tokenSource = new CancellationTokenSource();
        var subscription = await RegisterSubscription(httpContext, webSocket, queryName, parameters);

        await WaitForWebSocketClose(webSocket, tokenSource);
        await RemoveSubscription(httpContext, subscription);
    }

    private static async Task<Subscription> RegisterSubscription(HttpContext httpContext, WebSocket webSocket, string queryName, IEnumerable<QueryParameter> parameters)
    {
        var registerHandler = httpContext.RequestServices.GetService<IRegisterSubscriptionHandler>();

        var resultSender = new WebSocketResultSender(webSocket);
        var subscription = new Subscription
        {
            Name = $"ws-{Guid.NewGuid()}",
            Parameters = parameters.Select(x => new SubscriptionParameter { Name = x.Name, Values = x.Values }).ToList(),
            QueryName = queryName,
            ReportIfEmpty = false,
            Schedule = ParseSchedule(httpContext.Request.QueryString),
            Trigger = httpContext.Request.Query.Any(x => x.Key == "stream") ? "stream" : null,
            FormatterName = resultSender.Name
        };

        return await registerHandler.RegisterSubscriptionAsync(subscription, resultSender, httpContext.RequestAborted);
    }

    private static SubscriptionSchedule ParseSchedule(QueryString query)
    {
        var queryString = HttpUtility.ParseQueryString(query.ToString());
        var schedule = new SubscriptionSchedule
        {
            Second = queryString.Get("second") ?? string.Empty,
            Minute = queryString.Get("minute") ?? string.Empty,
            Hour = queryString.Get("hour") ?? string.Empty,
            Month = queryString.Get("month") ?? string.Empty,
            DayOfWeek = queryString.Get("dayOfWeek") ?? string.Empty,
            DayOfMonth = queryString.Get("dayOfMonth") ?? string.Empty
        };

        return schedule.IsEmpty() ? default : schedule;
    }

    private static Task RemoveSubscription(HttpContext httpContext, Subscription subscription)
    {
        var subscriptionRemover = httpContext.RequestServices.GetService<IDeleteSubscriptionHandler>();

        return subscriptionRemover.DeleteSubscriptionAsync(subscription.Name, CancellationToken.None);
    }

    private static async Task WaitForWebSocketClose(WebSocket webSocket, CancellationTokenSource tokenSource)
    {
        var arraySegment = new ArraySegment<byte>(new byte[8 * 1024]);

        while (!tokenSource.IsCancellationRequested)
        {
            await webSocket.ReceiveAsync(arraySegment, CancellationToken.None);

            if (webSocket.State == WebSocketState.CloseReceived)
            {
                tokenSource.Cancel();
            }
        }
    }
}
