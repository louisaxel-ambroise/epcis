using FasTnT.Domain.Commands.Capture;
using FasTnT.Formatter.v2_0.Json;
using MediatR;

namespace FasTnT.Host.Features.v2_0;

public record CaptureRequest(IRequest<CaptureEpcisRequestResponse> Request)
{
    public static async ValueTask<CaptureRequest> BindAsync(HttpContext context)
    {
        if (context.Request.ContentType.Contains("xml", StringComparison.OrdinalIgnoreCase))
        {
            throw new Exception("XML not supported yet for v2.0");
        }
        else
        {
            var epcisContext = context.Request.Headers.TryGetValue("GS1-Extensions", out var extensions)
                ? extensions.Select(x => x.Split('=', 2)).ToDictionary(x => x[0], x => x[1])
                : new();

            var request = await CaptureRequestParser.ParseAsync(context.Request.Body, epcisContext, context.RequestAborted);

            return new(request);
        }
    }
}

