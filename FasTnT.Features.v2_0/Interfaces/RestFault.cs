using FasTnT.Domain.Exceptions;
using FasTnT.Formatter.v2_0.Json.Formatters;

namespace FasTnT.Host.Features.v2_0.Interfaces;

public record RestFault(EpcisException Error) : IRestResponse
{
    public async Task ExecuteAsync(HttpContext context)
    {
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsync(JsonResponseFormatter.FormatError(Error));
    }
}
