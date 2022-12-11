using FasTnT.Domain.Model;
using FasTnT.Host.Features.v2_0.Communication.Json.Parsers;
using FasTnT.Host.Features.v2_0.Communication.Xml.Parsers;

namespace FasTnT.Host.Features.v2_0.Endpoints.Interfaces;

public record CaptureDocumentRequest(Request Request)
{
    public static async ValueTask<CaptureDocumentRequest> BindAsync(HttpContext context)
    {
        Request request;
        if (context.Request.ContentType.Contains("xml", StringComparison.OrdinalIgnoreCase))
        {
            request = await XmlCaptureRequestParser.ParseAsync(context.Request.Body, context.RequestAborted);
        }
        else
        {
            var headerContext = context.Request.Headers.TryGetValue("GS1-Extensions", out var extensions) ? extensions.ToArray() : Array.Empty<string>();
            var epcisContext = Namespaces.ParseHeader(headerContext);

            request = await JsonCaptureRequestParser.ParseDocumentAsync(context.Request.Body, epcisContext, context.RequestAborted);
        }

        return request;
    }

    public static implicit operator CaptureDocumentRequest(Request request) => new(request);
}
