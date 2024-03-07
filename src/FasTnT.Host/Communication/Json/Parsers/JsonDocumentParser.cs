using FasTnT.Domain.Exceptions;
using Json.Schema;
using System.Text.Json;

namespace FasTnT.Host.Communication.Json.Parsers;

public class JsonDocumentParser
{
    public static JsonDocumentParser Instance { get; } = new();
    private readonly JsonSchema _schema;

    private JsonDocumentParser()
    {
        var assembly = typeof(JsonDocumentParser).Assembly;
        using var reader = new StreamReader(assembly.GetManifestResourceStream("FasTnT.Host.Features.v2_0.Communication.Json.Schemas.epcis2_0.json"));

        _schema = JsonSchema.FromText(reader.ReadToEnd());
    }

    public async Task<JsonDocument> ParseAsync(Stream input, CancellationToken cancellationToken)
    {
        var document = await LoadDocument(input, cancellationToken).ConfigureAwait(false);

        if (!_schema.Evaluate(document).IsValid)
        {
            throw new EpcisException(ExceptionType.ValidationException, "JSON is invalid according to EPCIS 2.0 schema");
        }

        return document;
    }

    private static async Task<JsonDocument> LoadDocument(Stream input, CancellationToken cancellationToken)
    {
        try
        {
            return await JsonDocument.ParseAsync(input, default, cancellationToken);
        }
        catch
        {
            throw new EpcisException(ExceptionType.ValidationException, "JSON is invalid");
        }
    }
}
