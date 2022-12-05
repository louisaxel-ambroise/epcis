using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Events;

namespace FasTnT.Features.v2_0.Communication.Json.Parsers;

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
            CaptureDate = DateTimeOffset.UtcNow,
            DocumentTime = DateTimeOffset.UtcNow,
            SchemaVersion = "2.0",
            Events = new List<Event> { JsonEventParser.Create(document.RootElement, extensions).Parse() }
        };

        return request;
    }
}
