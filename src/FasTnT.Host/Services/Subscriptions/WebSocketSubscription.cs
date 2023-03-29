using FasTnT.Application;
using FasTnT.Application.Domain.Exceptions;
using FasTnT.Application.Domain.Model;
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
    private bool _running;
    private readonly object _monitor = new();
    private readonly List<int> _pendingRequests = new();
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
        EpcisEvents.OnRequestCaptured += RegisterRequest;
        Task.Run(() => WaitForWebSocketClose(_webSocket, cancellationToken), cancellationToken);

        try
        {
            cancellationToken.Register(Stop);
            _running = true;

            while (_running)
            {
                lock (_pendingRequests)
                {
                    if (_pendingRequests.Any())
                    {
                        var queryData = context
                            .QueryEvents(_parameters)
                            .Where(x => _pendingRequests.Contains(x.Request.Id))
                            .ToList();

                        SendQueryData(queryData, cancellationToken);
                    }

                    _pendingRequests.Clear();
                }

                lock (_monitor)
                {
                    _ = _schedule is null
                        ? Monitor.Wait(_monitor)
                        : Monitor.Wait(_monitor, SubscriptionSchedule.GetNextOccurence(_schedule, DateTime.UtcNow) - DateTime.UtcNow);
                }
            }
        }
        finally
        {
            EpcisEvents.OnRequestCaptured -= RegisterRequest;
        }
    }

    private void SendQueryData(List<Application.Domain.Model.Events.Event> queryData, CancellationToken cancellationToken)
    {
        if (!queryData.Any()) return;

        try
        {
            var response = new QueryResponse(_queryName, "webservice-subscription", queryData);
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

    private void RegisterRequest(object _, Request request)
    {
        lock (_pendingRequests)
        {
            _pendingRequests.Add(request.Id);
        }

        if (_schedule is null)
        {
            lock (_monitor)
            {
                Monitor.Pulse(_monitor);
            }
        }
    }

    private static Task Send(WebSocket webSocket, string content, CancellationToken cancellationToken)
    {
        var responseByteArray = Encoding.UTF8.GetBytes(content);

        return webSocket.State == WebSocketState.Open
            ? webSocket.SendAsync(responseByteArray, WebSocketMessageType.Text, true, cancellationToken)
            : Task.CompletedTask;
    }

    private async Task WaitForWebSocketClose(WebSocket webSocket, CancellationToken cancellationToken)
    {
        var arraySegment = new ArraySegment<byte>(new byte[8 * 1024]);

        while (!cancellationToken.IsCancellationRequested)
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
        }

        Stop();
    }

    private void Stop()
    {
        lock (_monitor) 
        { 
            _running = false; 
            Monitor.Pulse(_monitor); 
        }
    }
}
