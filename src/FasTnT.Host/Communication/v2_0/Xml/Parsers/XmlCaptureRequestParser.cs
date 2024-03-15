using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;

namespace FasTnT.Host.Communication.Xml.Parsers;

public static class XmlCaptureRequestParser
{
    public static async Task<Request> ParseAsync(Stream input, CancellationToken cancellationToken)
    {
        var document = await XmlDocumentParser.Instance.ParseAsync(input, cancellationToken);
        var request = XmlEpcisDocumentParser.Parse(document.Root);

        return request
            ?? throw new EpcisException(ExceptionType.ValidationException, $"Document with root '{document.Root.Name}' is not expected here.");
    }
}
