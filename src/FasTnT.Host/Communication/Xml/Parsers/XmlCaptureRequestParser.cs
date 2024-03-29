using FasTnT.Domain.Model;

namespace FasTnT.Host.Communication.Xml.Parsers;

public static class XmlCaptureRequestParser
{
    public static async Task<Request> ParseAsync(Stream input, CancellationToken cancellationToken)
    {
        var documentParser = await XmlDocumentParser.Instance.ParseAsync(input, cancellationToken);

        return documentParser.Parse();
    }
}
