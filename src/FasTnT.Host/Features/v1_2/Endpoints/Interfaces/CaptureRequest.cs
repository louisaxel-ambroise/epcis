using FasTnT.Application.Domain.Enumerations;
using FasTnT.Application.Domain.Exceptions;
using FasTnT.Application.Domain.Format.v1_2.Parsers;
using FasTnT.Application.Domain.Model;
using FasTnT.Host.Features.v1_2.Communication.Parsers;

namespace FasTnT.Host.Features.v1_2.Endpoints.Interfaces;

public record CaptureRequest(Request Request)
{
    public static async ValueTask<CaptureRequest> BindAsync(HttpContext context)
    {
        var document = await XmlDocumentParser.Instance.ParseAsync(context.Request.Body, context.RequestAborted);
        var request = XmlEpcisDocumentParser.Parse(document.Root);

        return request ?? throw new EpcisException(ExceptionType.ValidationException, $"Document with root '{document.Root.Name}' is not expected here.");
    }

    public static implicit operator CaptureRequest(Request request) => new(request);
}
