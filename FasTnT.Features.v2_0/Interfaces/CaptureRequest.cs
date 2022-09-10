using FasTnT.Domain.Commands.Capture;
using FasTnT.Features.v2_0.Communication.Xml.Parsers;
using FasTnT.Formatter.v2_0.Json;
using MediatR;

namespace FasTnT.Host.Features.v2_0;

public record CaptureRequest(IRequest<CaptureEpcisRequestResponse> Request)
{
    public static async ValueTask<CaptureRequest> BindAsync(HttpContext context)
    {
         var command = default(CaptureEpcisRequestCommand);

        if (context.Request.ContentType.Contains("xml", StringComparison.OrdinalIgnoreCase))
        {
            command = await XmlCaptureRequestParser.ParseAsync(context.Request.Body, context.RequestAborted);
        }
        else
        {
            var epcisContext = context.Request.Headers.TryGetValue("GS1-Extensions", out var extensions)
                ? extensions.Select(x => x.Split('=', 2)).ToDictionary(x => x[0], x => x[1])
                : new();

            command = await JsonCaptureRequestParser.ParseAsync(context.Request.Body, epcisContext, context.RequestAborted);
        }

        return new(command);
    }
}
