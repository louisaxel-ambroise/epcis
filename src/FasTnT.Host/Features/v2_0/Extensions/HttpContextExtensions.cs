using FasTnT.Application.Database;
using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Host.Services.Subscriptions;
using System.Net.WebSockets;
using System.Text;
using System.Web;

namespace FasTnT.Host.Features.v2_0.Extensions;

public static class WebSocketExtensions
{
    public static async Task SendAsync(this WebSocket webSocket, string content, CancellationToken cancellationToken)
    {
        var responseByteArray = Encoding.UTF8.GetBytes(content);

        if (webSocket.State == WebSocketState.Open) 
        {
            await webSocket.SendAsync(responseByteArray, WebSocketMessageType.Text, true, cancellationToken);
        }
    }

    public static async Task WaitForCompletion(this WebSocket webSocket, Action callback, CancellationToken cancellationToken)
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
                }
            }
            catch
            {
                // The loop will stop if the server is shutdown or socket closed
            }
        }

        callback();
    }
}

public static class HttpContextExtensions
{
    public static string PerPage => "perPage";
    public static string DefaultPageSize => "30";

    public static bool IsPaginated(this HttpContext context)
    {
        return context.Request.Query.Any(x => x.Key.Equals(PerPage));
    }

    public static int GetPerPageValue(this HttpContext context)
    {
        return int.Parse(context.Request.Query.TryGetValue(PerPage, out var value) ? value : DefaultPageSize);
    }

    public static string BuildNextLink(this HttpContext context, ICollection queryString)
    {
        return context.Request.Scheme + "://" + context.Request.Host + context.Request.Path + "?" + queryString;
    }

    public static async Task<IResult> HandleWebsocketAsync(this HttpContext context, string queryName, IEnumerable<QueryParameter> parameters)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var schedule = ParseSchedule(context);
        var subscriptionTask = new WebSocketSubscriptionTask(webSocket, queryName, parameters, schedule);

        using var scope = context.RequestServices.CreateScope();
        using var epcisContext = scope.ServiceProvider.GetService<EpcisContext>();

        var applicationLifetime = scope.ServiceProvider.GetService<IHostApplicationLifetime>();

        try
        {
            await subscriptionTask.Run(epcisContext, applicationLifetime.ApplicationStopping);
        }
        catch(Exception ex)
        {
            // A failure in the WebSocket exception can't be thrown further
            scope.ServiceProvider
                .GetService<ILogger<WebSocketSubscriptionTask>>()
                .LogError(ex, "An exception happened during websocket processing");
        }

        return Results.Empty;
    }

    public static SubscriptionSchedule ParseSchedule(HttpContext context)
    {
        var queryString = HttpUtility.ParseQueryString(context.Request.QueryString.ToString());
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
}
