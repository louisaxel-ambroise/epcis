using FasTnT.Domain.Queries;
using FasTnT.Formatter.v2_0.Json.Formatters;
using FasTnT.Formatter.Xml.Formatters;

namespace FasTnT.Host.Features.v2_0.Interfaces;

public record RestResponse(IEpcisResponse Response) : IRestResponse
{
    public async Task ExecuteAsync(HttpContext context)
    {
        var accept = context.Request.Headers.Accept.FirstOrDefault("application/json");

        // TODO: fix formatting (root/document)
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
}
