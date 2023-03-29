using FasTnT.Application.Domain.Enumerations;
using FasTnT.Application.Domain.Exceptions;
using FasTnT.Application.Domain.Model;
using FasTnT.Host.Features.v2_0.Communication;
using FasTnT.Host.Features.v2_0.Communication.Parsers;

namespace FasTnT.Host.Features.v2_0.Endpoints.Interfaces;

public record CaptureDocumentRequest(Request Request)
{
    public static async ValueTask<CaptureDocumentRequest> BindAsync(HttpContext context)
    {
        Request request;

        if (context.Request.ContentType.Contains("xml", StringComparison.OrdinalIgnoreCase))
        {
            var document = await XmlDocumentParser.Instance.ParseAsync(context.Request.Body, context.RequestAborted);
            request = XmlEpcisDocumentParser.Parse(document.Root);
        }
        else
        {
            var headerContext = context.Request.Headers.TryGetValue("GS1-Extensions", out var extensions) ? extensions.ToArray() : Array.Empty<string>();
            var epcisContext = Namespaces.ParseHeader(headerContext);

            var document = await JsonDocumentParser.Instance.ParseAsync(context.Request.Body, context.RequestAborted);
            request = JsonEpcisDocumentParser.Parse(document, epcisContext);
        }

        return request ?? throw new EpcisException(ExceptionType.ValidationException, $"Document in invalid according to EPCIS specification.");
    }

    public static implicit operator CaptureDocumentRequest(Request request) => new(request);
}
