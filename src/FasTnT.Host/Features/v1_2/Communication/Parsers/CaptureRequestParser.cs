using FasTnT.Application.Domain.Exceptions;
using FasTnT.Application.Domain.Model;

namespace FasTnT.Host.Features.v1_2.Communication.Parsers;

public static class CaptureRequestParser
{
    public static async Task<Request> ParseAsync(Stream input, CancellationToken cancellationToken)
    {
        var document = await XmlDocumentParser.Instance.ParseAsync(input, cancellationToken);
        var request = XmlEpcisDocumentParser.Parse(document.Root);

        return request ?? throw new EpcisException(ExceptionType.ValidationException, $"Document with root '{document.Root.Name}' is not expected here.");
    }
}
