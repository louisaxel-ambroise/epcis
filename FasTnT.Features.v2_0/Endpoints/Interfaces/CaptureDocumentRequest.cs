using FasTnT.Domain.Commands.Capture;
using FasTnT.Features.v2_0.Communication.Xml.Parsers;
using FasTnT.Formatter.v2_0.Json;
using MediatR;

namespace FasTnT.Host.Features.v2_0;

public record CaptureDocumentRequest(IRequest<CaptureEpcisRequestResponse> Request)
{
    public static async ValueTask<CaptureDocumentRequest> BindAsync(HttpContext context)
    {
        CaptureEpcisRequestCommand command;

        if (context.Request.ContentType.Contains("xml", StringComparison.OrdinalIgnoreCase))
        {
            command = await XmlCaptureRequestParser.ParseAsync(context.Request.Body, context.RequestAborted);
        }
        else
        {
            var headerContext = context.Request.Headers.TryGetValue("GS1-Extensions", out var extensions) ? extensions.ToArray() : Array.Empty<string>();
            var epcisContext = Namespaces.ParseHeader(headerContext);

            command = await JsonCaptureRequestParser.ParseDocumentAsync(context.Request.Body, epcisContext, context.RequestAborted);
        }

        return new(command);
    }
}
