using FasTnT.Application.Database;
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

    public static Task<IResult> HandleWebsocketAsync(this HttpContext context, string queryName, IEnumerable<QueryParameter> parameters)
    {
        return context.WebSockets.AcceptWebSocketAsync().ContinueWith(x =>
        {
            using var scope = context.RequestServices.CreateScope();
            using var epcisContext = scope.ServiceProvider.GetService<EpcisContext>();

            var applicationLifetime = scope.ServiceProvider.GetService<IHostApplicationLifetime>();
            var schedule = ParseSchedule(context);
            var subscriptionTask = new WebSocketSubscriptionTask(x.Result, queryName, parameters, schedule);

            subscriptionTask.Run(epcisContext, applicationLifetime.ApplicationStopping);

            return Results.Empty;
        });
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
