using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Features.v2_0.Communication.Json.Formatters;

namespace FasTnT.Features.v2_0.Endpoints.Interfaces.Utils;

public record RestFault(EpcisException Error) : IResult
{
    public async Task ExecuteAsync(HttpContext context)
    {
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsync(JsonResponseFormatter.FormatError(Error));
    }
}
