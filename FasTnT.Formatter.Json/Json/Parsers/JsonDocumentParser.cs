using System.Text.Json;

namespace FasTnT.Formatter.v2_0.Json;

public class JsonDocumentParser
{
    public static JsonDocumentParser Instance { get; } = new();

    private JsonDocumentParser()
    {
    }

    public static async Task<JsonDocument> ParseAsync(Stream input, CancellationToken cancellationToken)
    {
        var document = await LoadDocument(input, cancellationToken).ConfigureAwait(false);

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
            throw new FormatException("JSON is invalid");
        }
    }
}
