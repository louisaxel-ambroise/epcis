using FasTnT.Domain.Model;
using FasTnT.Features.v1_2.Communication.Parsers;
using Microsoft.AspNetCore.Http;

namespace FasTnT.Features.v1_2.Endpoints.Interfaces;

public record CaptureRequest(Request Request)
{
    public static async ValueTask<CaptureRequest> BindAsync(HttpContext context)
    {
        return await CaptureRequestParser.ParseAsync(context.Request.Body, context.RequestAborted);
    }

    public static implicit operator CaptureRequest(Request request) => new(request);
}
