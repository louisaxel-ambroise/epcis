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
using System.Web;

namespace FasTnT.Host.Services.Subscriptions;

public class WebSocketSubscriptionTask
{
    public readonly CancellationTokenSource TokenSource = new();
    private readonly object _monitor = new();
    private readonly List<int> pendingRequests = new();
    private readonly EpcisContext _context;
    private readonly WebSocket _webSocket;
    private readonly SubscriptionSchedule _schedule;

    private WebSocketSubscriptionTask(EpcisContext context, WebSocket webSocket, SubscriptionSchedule schedule)
    {
        _context = context;
        _webSocket = webSocket;
        _schedule = schedule;
    }

    public static async Task<WebSocketSubscriptionTask> CreateAsync(HttpContext httpContext)
    {
        var context = httpContext.RequestServices.GetService<EpcisContext>();
        var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();
        var schedule = ParseSchedule(httpContext.Request.QueryString);
        var subscriptionTask = new WebSocketSubscriptionTask(context, webSocket, schedule);

        var applicationLifetime = httpContext.RequestServices.GetService<IHostApplicationLifetime>();
        applicationLifetime.ApplicationStopping.Register(subscriptionTask.TokenSource.Cancel);

        return subscriptionTask;
    }

    public IResult Run(string queryName, IEnumerable<QueryParameter> parameters)
    {
        EpcisEvents.OnRequestCaptured += RegisterRequest;
        _ = Task.Run(() => WaitForWebSocketClose(_webSocket, TokenSource));

        try
        {
            TokenSource.Token.Register(() => { lock (_monitor) { Monitor.Pulse(_monitor); } });

            while (!TokenSource.IsCancellationRequested)
            {
                lock (pendingRequests)
                {
                    if (!pendingRequests.Any())
                    {
                        continue;
                    }

                    var queryData = _context
                        .QueryEvents(parameters)
                        .Where(x => pendingRequests.Contains(x.Request.Id))
                        .ToList();

                    if (!queryData.Any())
                    {
                        continue;
                    }

                    try
                    {
                        var response = new QueryResponse(queryName, "webservice-subscription", queryData);
                        var formattedResponse = JsonResponseFormatter.Format(new QueryResult(response));

                        Send(_webSocket, formattedResponse, TokenSource.Token).Wait(TokenSource.Token);
                    }
                    catch (Exception ex)
                    {
                        var epcisError = ex as EpcisException ?? EpcisException.Default;
                        var formattedError = JsonResponseFormatter.FormatError(epcisError);

                        Send(_webSocket, formattedError, TokenSource.Token).Wait(TokenSource.Token);
                    }

                    pendingRequests.Clear();
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

        return Results.Empty;
    }

    private void RegisterRequest(object _, Request request)
    {
        pendingRequests.Add(request.Id);

        if (_schedule is not null) return;

        lock (_monitor)
        {
            Monitor.Pulse(_monitor);
        }
    }

    private static Task Send(WebSocket webSocket, string content, CancellationToken cancellationToken)
    {
        var responseByteArray = Encoding.UTF8.GetBytes(content);

        return webSocket.SendAsync(responseByteArray, WebSocketMessageType.Text, true, cancellationToken);
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
            await webSocket.ReceiveAsync(arraySegment, tokenSource.Token);

            if (webSocket.State == WebSocketState.CloseReceived)
            {
                tokenSource.Cancel();
            }
        }
    }
}
