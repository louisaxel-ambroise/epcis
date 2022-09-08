using FasTnT.Domain.Queries;
using FasTnT.Formatter.v2_0.Json.Formatters;

namespace FasTnT.Host.Features.v2_0.Interfaces;

public record RestResponse(IEpcisResponse Response) : IRestResponse
{
    public async Task ExecuteAsync(HttpContext context)
    {
        var formattedResponse = JsonResponseFormatter.Format(Response);

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(formattedResponse);
    }
}
