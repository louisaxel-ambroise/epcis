using FasTnT.Domain.Commands.Capture;
using FasTnT.Domain.Exceptions;

namespace FasTnT.Formatter.v2_0.Json;

public static class CaptureRequestParser
{
    public static async Task<CaptureEpcisRequestCommand> ParseAsync(Stream input, IDictionary<string, string> extensions, CancellationToken cancellationToken)
    {
        var document = await JsonDocumentParser.ParseAsync(input, cancellationToken);
        var request = JsonEpcisDocumentParser.Parse(document, extensions);

        return request != default
                ? new CaptureEpcisRequestCommand { Request = request }
                : throw new EpcisException(ExceptionType.ValidationException, $"JSON is not a valid EPCIS request.");
    }
}
