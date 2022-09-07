using FasTnT.Domain.Commands.Capture;
using FasTnT.Domain.Exceptions;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Formatter.Json;

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
