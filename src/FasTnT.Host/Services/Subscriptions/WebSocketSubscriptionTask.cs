using FasTnT.Application;
using FasTnT.Application.Database;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Host.Features.v2_0.Communication.Json.Formatters;
using FasTnT.Host.Features.v2_0.Endpoints.Interfaces;
using FasTnT.Host.Features.v2_0.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace FasTnT.Host.Services.Subscriptions;

public class WebSocketSubscriptionTask
{
    private readonly object _monitor = new();
    private readonly WebSocket _webSocket;
    private readonly string _queryName;
    private readonly IEnumerable<QueryParameter> _parameters;
    private readonly SubscriptionSchedule _schedule;

    public WebSocketSubscriptionTask(WebSocket webSocket, string queryName, IEnumerable<QueryParameter> parameters, SubscriptionSchedule schedule)
    {
        _webSocket = webSocket;
        _queryName = queryName;
        _parameters = parameters;
        _schedule = schedule;
    }

    public async Task Run(EpcisContext context, CancellationToken cancellationToken)
    {
        var running = true;
        var lastCaptureTime = DateTime.UtcNow;
        var lastCaptureIds = Array.Empty<int>();
        var nextExecutionTime = DateTime.UtcNow;
        var handleRequest = (Request _) => Pulse(() => { });
        _ = Task.Run(() => _webSocket.WaitForCompletion(() => Pulse(() => running = false), cancellationToken), CancellationToken.None);

        EpcisEvents.OnRequestCaptured += handleRequest;

        try
        {
            while (running)
            {
                if (_schedule is null || nextExecutionTime <= DateTime.UtcNow)
                {
                    var eventIds = await context
                        .QueryEvents(_parameters)
                        .Where(x => x.Request.CaptureTime >= lastCaptureTime && !lastCaptureIds.Contains(x.Id))
                        .Select(x => x.Id)
                        .ToListAsync(cancellationToken);

                    if (eventIds.Any())
                    {
                        var events = context.Set<Event>()
                            .Where(x => eventIds.Contains(x.Id))
                            .AsEnumerable()
                            .OrderBy(x => eventIds.IndexOf(x.Id))
                            .ToList();

                        // Use the min capture time of the retrieved events as lastCaptureTime
                        // reduces the risk of missing concurrent requests.
                        lastCaptureTime = events.Min(e => e.CaptureTime);
                        lastCaptureIds = eventIds.ToArray();

                        await SendQueryDataAsync(events, cancellationToken);
                    }
                    nextExecutionTime = GetNextExecutionTime(_schedule, DateTime.UtcNow);
                }

                lock (_monitor)
                {
                    var waitTimeout = Math.Min(5000, (nextExecutionTime - DateTime.UtcNow).TotalMilliseconds);
                    Monitor.Wait(_monitor, TimeSpan.FromMilliseconds(waitTimeout));
                }
            }
        }
        finally
        {
            EpcisEvents.OnRequestCaptured -= handleRequest;
        }
    }

    private void Pulse(Action action)
    {
        lock (_monitor)
        {
            action();
            Monitor.Pulse(_monitor);
        }
    }

    private static DateTime GetNextExecutionTime(SubscriptionSchedule schedule, DateTime startDate)
    {
        return schedule is null
            ? DateTime.MaxValue
            : SubscriptionSchedule.GetNextOccurence(schedule, startDate);
    }

    private async Task SendQueryDataAsync(List<Event> queryData, CancellationToken cancellationToken)
    {
        // Send the events by batch to avoid long processing on server side
        for (var i = 0; i < queryData.Count; i+=50)
        {
            var data = queryData.Skip(i).Take(50).ToList();
            try
            {
                var response = new QueryResponse(_queryName, "websocket-subscription", data);
                var formattedResponse = JsonResponseFormatter.Format(new QueryResult(response));

                await _webSocket.SendAsync(formattedResponse, cancellationToken);
            }
            catch (Exception ex)
            {
                var epcisError = ex as EpcisException ?? EpcisException.Default;
                var formattedError = JsonResponseFormatter.FormatError(epcisError);

                await _webSocket.SendAsync(formattedError, cancellationToken);
            }
        }
    }
}
