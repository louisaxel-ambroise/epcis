using FasTnT.Host.Features.v2_0.Communication.Formatters;
using FasTnT.Host.Features.v2_0.Extensions;
using System.Web;

namespace FasTnT.Host.Features.v2_0.Endpoints.Interfaces.Utils;

public record RestResponse<T>(T Response) : IResult
{
    public async Task ExecuteAsync(HttpContext context)
    {
        var accept = context.Request.Headers.Accept.FirstOrDefault("application/json");

        if (Response is IPaginableResult paginableResult)
        {
            SetNextPageToken(context, paginableResult);
        }

        if (accept.Contains("xml", StringComparison.OrdinalIgnoreCase))
        {
            var formattedResponse = XmlResponseFormatter.Format(Response);

            context.Response.ContentType = "application/xml";
            await context.Response.WriteAsync(formattedResponse, context.RequestAborted);
        }
        else
        {
            var formattedResponse = JsonResponseFormatter.Format(Response);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(formattedResponse, context.RequestAborted);
        }
    }

    private static void SetNextPageToken(HttpContext context, IPaginableResult paginableResult)
    {
        var perPage = context.GetPerPageValue();

        if (paginableResult.ElementsCount >= perPage)
        {
            var queryString = HttpUtility.ParseQueryString(context.Request.QueryString.ToString());
            var originalToken = int.Parse(queryString.Get("nextPageToken") ?? "0");

            queryString.Set("nextPageToken", (originalToken + perPage).ToString());
            context.Response.Headers.Add("link", $"<{context.BuildNextLink(queryString)}>;rel=next");
        }
    }
}