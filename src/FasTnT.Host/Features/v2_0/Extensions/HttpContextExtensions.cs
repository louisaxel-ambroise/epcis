using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Host.Services.Subscriptions;
using System.Web;

namespace FasTnT.Host.Features.v2_0.Extensions;

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
        var scheduler = ParseSchedule(context);
        var subscriptionTask = new WebSocketSubscriptionTask(webSocket, queryName, parameters, scheduler);
        var applicationLifetime = context.RequestServices.GetService<IHostApplicationLifetime>();

        try
        {
            await subscriptionTask.Run(context.RequestServices, applicationLifetime.ApplicationStopping);
        }
        catch (Exception ex)
        {
            // A failure in the WebSocket exception can't be thrown further
            context.RequestServices
                .GetService<ILogger<WebSocketSubscriptionTask>>()
                .LogError(ex, "An exception happened during websocket processing");
        }

        return Results.Empty;
    }

    public static ISubscriptionScheduler ParseSchedule(HttpContext context)
    {
        var queryString = HttpUtility.ParseQueryString(context.Request.QueryString.ToString());
        var schedule = new SubscriptionSchedule
        {
            Second = queryString.Get("second"),
            Minute = queryString.Get("minute"),
            Hour = queryString.Get("hour"),
            Month = queryString.Get("month"),
            DayOfWeek = queryString.Get("dayOfWeek"),
            DayOfMonth = queryString.Get("dayOfMonth")
        };

        return ISubscriptionScheduler.Get(schedule);
    }
}
