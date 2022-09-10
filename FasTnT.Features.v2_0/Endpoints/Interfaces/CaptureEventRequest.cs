using FasTnT.Domain.Commands.Capture;
using FasTnT.Features.v2_0.Communication.Json.Parsers;
using MediatR;

namespace FasTnT.Features.v2_0.Endpoints.Interfaces;

public record CaptureEventRequest(IRequest<CaptureEpcisRequestResponse> Request)
{
    public static async ValueTask<CaptureEventRequest> BindAsync(HttpContext context)
    {
        var headerContext = context.Request.Headers.TryGetValue("GS1-Extensions", out var extensions) ? extensions.ToArray() : Array.Empty<string>();
        var epcisContext = Namespaces.ParseHeader(headerContext);

        var command = await JsonCaptureRequestParser.ParseEventAsync(context.Request.Body, epcisContext, context.RequestAborted);

        return new(command);
    }
}
