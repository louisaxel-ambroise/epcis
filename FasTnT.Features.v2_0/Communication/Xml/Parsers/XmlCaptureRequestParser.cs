using FasTnT.Domain.Commands.Capture;
using FasTnT.Domain.Infrastructure.Exceptions;

namespace FasTnT.Features.v2_0.Communication.Xml.Parsers;

public static class XmlCaptureRequestParser
{
    public static async Task<CaptureEpcisRequestCommand> ParseAsync(Stream input, CancellationToken cancellationToken)
    {
        var document = await XmlDocumentParser.Instance.ParseAsync(input, cancellationToken);
        var request = XmlEpcisDocumentParser.Parse(document.Root);

        return request != default
                ? new CaptureEpcisRequestCommand { Request = request }
                : throw new EpcisException(ExceptionType.ValidationException, $"Document with root '{document.Root.Name}' is not expected here.");
    }
}
