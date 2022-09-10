using FasTnT.Domain.Commands.Capture;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;
using System.Text.Json;

namespace FasTnT.Formatter.v2_0.Json;

public static class JsonCaptureRequestParser
{
    public static async Task<CaptureEpcisRequestCommand> ParseDocumentAsync(Stream input, Namespaces extensions, CancellationToken cancellationToken)
    {
        var document = await JsonDocumentParser.Instance.ParseAsync(input, cancellationToken);
        var request = JsonEpcisDocumentParser.Parse(document, extensions);

        return request != default
                ? new CaptureEpcisRequestCommand { Request = request }
                : throw new EpcisException(ExceptionType.ValidationException, $"JSON is not a valid EPCIS request.");
    }

    public static async Task<CaptureEpcisRequestCommand> ParseEventAsync(Stream input, Namespaces extensions, CancellationToken cancellationToken)
    {
        var document = await JsonDocumentParser.Instance.ParseAsync(input, cancellationToken);
        //var document = await JsonDocument.ParseAsync(input, default, cancellationToken);
        var request = new Request
        {
            CaptureDate = DateTime.UtcNow,
            DocumentTime = DateTime.UtcNow,
            SchemaVersion = "2.0",
            Events = new List<Event> { JsonEventParser.Create(document.RootElement, extensions).Parse() }
        };

        return new CaptureEpcisRequestCommand { Request = request };
    }
}
