using FasTnT.Application.Domain.Exceptions;
using FasTnT.Application.Domain.Model.Queries;
using FasTnT.Application.Domain.Model.Subscriptions;
using FasTnT.Application.Handlers;
using FasTnT.Application.Services.Storage;
using FasTnT.Application.Services.Subscriptions;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Web;

namespace FasTnT.Host.Features.Subscriptions;

public class WebSocketSubscriptionContext : ISubscriptionContext
{
    private readonly Subscription _subscription;
    private readonly IResultSender _resultSender;
    private List<int> _pendingRequests = new();

    public WebSocketSubscriptionContext(Subscription subscription, IResultSender resultSender)
    {
        _subscription = subscription;
        _resultSender = resultSender;

        EpcisEvents.Instance.OnRequestCaptured += (_, request) => _pendingRequests.Add(request.Id);
    }

    public async static Task SubscribeAsync(HttpContext httpContext, string queryName, IEnumerable<QueryParameter> parameters)
    {
        using var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();

        var subscription = ParseSubscription(httpContext, queryName, parameters);
        var resultSender = new WebSocketResultSender(webSocket);
        var tokenSource = new CancellationTokenSource(); 
        
        EpcisEvents.Instance.Register(new WebSocketSubscriptionContext(subscription, resultSender));

        await WaitForWebSocketClose(webSocket, tokenSource);

        EpcisEvents.Instance.Remove(subscription.Name);
    }

    public string Name => _subscription.Name;
    public string Trigger => _subscription.Trigger;

    public bool IsScheduled() => _subscription.Trigger is null;

    public async Task ExecuteAsync(EpcisContext context, DateTime executionTime, CancellationToken cancellationToken)
    {
        var resultsSent = false;
        var executionRecord = new SubscriptionExecutionRecord { ExecutionTime = executionTime, ResultsSent = true, Successful = true, SubscriptionId = _subscription.Id };

        try
        {
            var response = new QueryResponse(_subscription.QueryName, _subscription.Name, QueryData.Empty);

            if (_pendingRequests.Any())
            {
                var queryData = await context
                    .QueryEvents(_subscription.Parameters)
                    .Where(x => _pendingRequests.Contains(x.Request.Id))
                    .ToListAsync(cancellationToken);

                response = new QueryResponse(_subscription.QueryName, _subscription.Name, queryData);
            }

            resultsSent = await _resultSender.SendResultAsync(_subscription, response, cancellationToken);
        }
        catch (EpcisException ex)
        {
            resultsSent = await _resultSender.SendErrorAsync(_subscription, ex, cancellationToken);
        }

        if (resultsSent)
        {
            _pendingRequests.Clear();
        }
        else
        {
            executionRecord.Successful = false;
            executionRecord.Reason = "Failed to send subscription result";
        }
    }

    public DateTime GetNextOccurence(DateTime executionTime)
    {
        return SubscriptionSchedule.GetNextOccurence(_subscription.Schedule, executionTime);
    }

    private static Subscription ParseSubscription(HttpContext httpContext, string queryName, IEnumerable<QueryParameter> parameters)
    {
        return new Subscription
        {
            Name = $"ws-{Guid.NewGuid()}",
            QueryName = queryName,
            Parameters = parameters.ToList(),
            Schedule = ParseSchedule(httpContext.Request.QueryString),
            Trigger = httpContext.Request.Query.Any(x => x.Key == "stream") ? "stream" : null
        };
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
