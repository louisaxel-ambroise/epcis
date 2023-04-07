using FasTnT.Application;
using FasTnT.Application.Database;
using FasTnT.Application.Services.Subscriptions;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;
using FasTnT.Host.Features.v2_0.Extensions;
using FasTnT.Host.Subscriptions.Formatters;
using FasTnT.Host.Subscriptions.Schedulers;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace FasTnT.Host.Subscriptions.Jobs;

public class WebSocketSubscriptionJob
{
    private readonly WebSocket _webSocket;
    private readonly string _queryName;
    private readonly IEnumerable<QueryParameter> _parameters;
    private readonly SubscriptionScheduler _scheduler;

    public WebSocketSubscriptionJob(WebSocket webSocket, string queryName, IEnumerable<QueryParameter> parameters, SubscriptionScheduler scheduler)
    {
        _webSocket = webSocket;
        _queryName = queryName;
        _parameters = parameters;
        _scheduler = scheduler;
    }

    public async Task RunAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var bufferRequestIds = Array.Empty<int>();
        var minRecordDate = DateTime.UtcNow;

        EpcisEvents.OnRequestCaptured += _scheduler.OnRequestCaptured;

        try
        {
            while (!_scheduler.Stopped)
            {
                var executionDate = DateTime.UtcNow;

                if (_scheduler.IsDue())
                {
                    try
                    {
                        var parameters = _parameters.Union(new[]
                        {
                            QueryParameter.Create("GE_recordTime", minRecordDate.Subtract(TimeSpan.FromSeconds(10)).ToString())
                        });

                        using var scope = serviceProvider.CreateScope();
                        
                        var runner = scope.ServiceProvider.GetService<SubscriptionRunner>();
                        var result = await runner.ExecuteAsync(new(parameters, bufferRequestIds), cancellationToken);

                        if (result.Successful)
                        {
                            if (result.Events.Any())
                            {
                                await SendQueryDataAsync(result.Events, cancellationToken);
                            }

                            bufferRequestIds = result.RequestIds.ToArray();
                            minRecordDate = executionDate;
                        }
                    }
                    finally
                    {
                        _scheduler.ComputeNextExecution(executionDate);
                    }
                }

                _scheduler.WaitForNotification();
            }
        }
        finally
        {
            EpcisEvents.OnRequestCaptured -= _scheduler.OnRequestCaptured;
        }
    }

    private async Task SendQueryDataAsync(List<Event> queryData, CancellationToken cancellationToken)
    {
        var response = new QueryResponse(_queryName, queryData);
        var formattedResponse = JsonSubscriptionFormatter.Instance.FormatResult("ws-subscription", response);

        await _webSocket.SendAsync(formattedResponse, cancellationToken);
    }
}
