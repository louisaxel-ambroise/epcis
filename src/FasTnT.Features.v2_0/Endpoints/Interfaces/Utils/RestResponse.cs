using FasTnT.Features.v2_0.Communication.Json.Formatters;
using FasTnT.Features.v2_0.Communication.Xml.Formatters;

namespace FasTnT.Features.v2_0.Endpoints.Interfaces.Utils;

public record RestResponse<T>(T Response) : IResult
{
    public async Task ExecuteAsync(HttpContext context)
    {
        var accept = context.Request.Headers.Accept.FirstOrDefault("application/json");

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
