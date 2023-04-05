using FasTnT.Application.Database;
using FasTnT.Application.Services.Notifications;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;
using FasTnT.Host.Features.v2_0.Extensions;
using FasTnT.Host.Services.Subscriptions.Formatters;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace FasTnT.Host.Services.Subscriptions;

public class WebSocketSubscriptionTask
{
    private readonly object _monitor = new();
    private readonly WebSocket _webSocket;
    private readonly string _queryName;
    private readonly IEnumerable<QueryParameter> _parameters;
    private readonly ISubscriptionScheduler _scheduler;

    public WebSocketSubscriptionTask(WebSocket webSocket, string queryName, IEnumerable<QueryParameter> parameters, ISubscriptionScheduler scheduler)
    {
        _webSocket = webSocket;
        _queryName = queryName;
        _parameters = parameters;
        _scheduler = scheduler;
    }

    public async Task Run(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var running = true;
        var bufferRequestIds = Array.Empty<int>();
        var nextExecutionTime = DateTime.UtcNow;
        var minRecordDate = DateTime.UtcNow;
        var handleRequest = (Request _) => Pulse(() => { });
        _ = Task.Run(() => _webSocket.WaitForCompletion(() => Pulse(() => running = false), cancellationToken), CancellationToken.None);

        var notifier = serviceProvider.GetService<INotificationReceiver>();
        notifier.OnRequestCaptured += handleRequest;

        try
        {
            while (running)
            {
                var executionDate = DateTime.UtcNow;

                // TODO: check for stream websockets
                if (nextExecutionTime <= executionDate)
                {
                    var parameters = _parameters.Union(new[]
                    {
                        QueryParameter.Create("GE_recordTime", minRecordDate.Subtract(TimeSpan.FromSeconds(10)).ToString())
                    });

                    using var scope = serviceProvider.CreateScope();
                    using var context = scope.ServiceProvider.GetService<EpcisContext>();

                    var pendingEvents = await context.QueryEvents(parameters)
                        .Select(x => new { x.Id, RequestId = x.Request.Id })
                        .ToListAsync(cancellationToken);
                    var eventIds = pendingEvents.Where(x => !bufferRequestIds.Contains(x.RequestId)).Select(x => x.Id);

                    if (eventIds.Any())
                    {
                        var events = await context.Set<Event>()
                            .Where(x => eventIds.Contains(x.Id))
                            .ToListAsync(cancellationToken);

                        await SendQueryDataAsync(events, cancellationToken);
                    }

                    bufferRequestIds = pendingEvents.Select(x => x.RequestId).Distinct().ToArray();
                    minRecordDate = executionDate;
                    nextExecutionTime = _scheduler.GetNextExecution(executionDate);
                }

                lock (_monitor)
                {
                    var delay = nextExecutionTime - DateTime.UtcNow >= TimeSpan.FromHours(1)
                        ? TimeSpan.FromHours(1)
                        : nextExecutionTime - DateTime.UtcNow;

                    Monitor.Wait(_monitor, delay);
                }
            }
        }
        catch
        {
            notifier.OnRequestCaptured -= handleRequest;
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

    private async Task SendQueryDataAsync(List<Event> queryData, CancellationToken cancellationToken)
    {
        var response = new QueryResponse(_queryName, queryData);
        var formattedResponse = JsonSubscriptionFormatter.Instance.FormatResult("ws-subscription", response);

        await _webSocket.SendAsync(formattedResponse, cancellationToken);
    }
}
