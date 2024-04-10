using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;

namespace FasTnT.Host.Features.v2_0.Communication.Json.Parsers;

public static class JsonCaptureRequestParser
{
    public static async Task<Request> ParseDocumentAsync(Stream input, Namespaces extensions, CancellationToken cancellationToken)
    {
        var document = await JsonDocumentParser.Instance.ParseAsync(input, cancellationToken);
        var request = JsonEpcisDocumentParser.Parse(document, extensions);

        return request
            ?? throw new EpcisException(ExceptionType.ValidationException, $"JSON is not a valid EPCIS request.");
    }

    public static async Task<Request> ParseEventAsync(Stream input, Namespaces extensions, CancellationToken cancellationToken)
    {
        var document = await JsonDocumentParser.Instance.ParseAsync(input, cancellationToken);
        var request = new Request
        {
            DocumentTime = DateTime.UtcNow,
            SchemaVersion = "2.0",
            Events = [JsonEventParser.Create(document.RootElement, extensions).Parse()]
        };

        return request;
    }
}
