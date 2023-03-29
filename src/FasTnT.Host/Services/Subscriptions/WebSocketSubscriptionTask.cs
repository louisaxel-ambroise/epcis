using FasTnT.Application;
using FasTnT.Application.Domain.Exceptions;
using FasTnT.Application.Domain.Model;
using FasTnT.Application.Domain.Model.Events;
using FasTnT.Application.Domain.Model.Queries;
using FasTnT.Application.Domain.Model.Subscriptions;
using FasTnT.Application.Services.Storage;
using FasTnT.Host.Features.v2_0.Communication;
using FasTnT.Host.Features.v2_0.Endpoints.Interfaces;
using System.Net.WebSockets;
using System.Text;

namespace FasTnT.Host.Services.Subscriptions;

public class WebSocketSubscriptionTask
{
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

    public void Run(EpcisContext context, CancellationToken cancellationToken)
    {
        var running = true;
        var monitor = new object();
        var pendingRequests = new List<int>(); 
        var registerRequest = (Request request) => EnqueueRequest(request, pendingRequests, monitor);
        var websocketRunning = Task.Run(() => WaitForWebSocketClose(_webSocket, cancellationToken), cancellationToken).ContinueWith(_ =>
        {
            lock (monitor)
            {
                running = false;
                Monitor.Pulse(monitor);
            }
        }, CancellationToken.None);

        EpcisEvents.OnRequestCaptured += registerRequest;

        try
        {
            while (running)
            {
                lock (pendingRequests)
                {
                    if (pendingRequests.Any())
                    {
                        var queryData = context
                            .QueryEvents(_parameters)
                            .Where(x => pendingRequests.Contains(x.Request.Id))
                            .ToList();

                        SendQueryData(queryData, cancellationToken);
                    }

                    pendingRequests.Clear();
                }

                lock (monitor)
                {
                    _ = _schedule is null
                        ? Monitor.Wait(monitor)
                        : Monitor.Wait(monitor, SubscriptionSchedule.GetNextOccurence(_schedule, DateTime.UtcNow) - DateTime.UtcNow);
                }
            }
        }
        finally
        {
            EpcisEvents.OnRequestCaptured -= registerRequest;
        }
    }

    private void EnqueueRequest(Request request, List<int> pendingRequests, object monitor)
    {
        lock (pendingRequests)
        {
            pendingRequests.Add(request.Id);
        }

        if (_schedule is null)
        {
            lock (monitor)
            {
                Monitor.Pulse(monitor);
            }
        }
    }

    private void SendQueryData(List<Event> queryData, CancellationToken cancellationToken)
    {
        if (!queryData.Any())
        {
            return;
        }

        try
        {
            var response = new QueryResponse(_queryName, "websocket-subscription", queryData);
            var formattedResponse = JsonResponseFormatter.Format(new QueryResult(response));

            Send(_webSocket, formattedResponse, cancellationToken).Wait(cancellationToken);
        }
        catch (Exception ex)
        {
            var epcisError = ex as EpcisException ?? EpcisException.Default;
            var formattedError = JsonResponseFormatter.FormatError(epcisError);

            Send(_webSocket, formattedError, cancellationToken).Wait(cancellationToken);
        }
    }

    private static Task Send(WebSocket webSocket, string content, CancellationToken cancellationToken)
    {
        var responseByteArray = Encoding.UTF8.GetBytes(content);

        return webSocket.State == WebSocketState.Open
            ? webSocket.SendAsync(responseByteArray, WebSocketMessageType.Text, true, cancellationToken)
            : Task.CompletedTask;
    }

    private static async Task WaitForWebSocketClose(WebSocket webSocket, CancellationToken cancellationToken)
    {
        var arraySegment = new ArraySegment<byte>(new byte[8 * 1024]);

        while (!cancellationToken.IsCancellationRequested && webSocket.State == WebSocketState.Open)
        {
            try
            {
                await webSocket.ReceiveAsync(arraySegment, cancellationToken);

                if (webSocket.State == WebSocketState.CloseReceived)
                {
                    await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, cancellationToken);
                    break;
                }
            }
            catch
            {
                break;
            }
        };
    }
}
