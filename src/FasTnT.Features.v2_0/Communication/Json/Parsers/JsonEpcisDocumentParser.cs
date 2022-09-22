using FasTnT.Domain.Model;
using System.Text.Json;

namespace FasTnT.Features.v2_0.Communication.Json.Parsers;

public static class JsonEpcisDocumentParser
{
    public static Request Parse(JsonDocument document, Namespaces extensions)
    {
        if (document.RootElement.TryGetProperty("@context", out JsonElement context))
        {
            extensions = extensions.Merge(Namespaces.Parse(context));
        }

        var schemaVersion = document.RootElement.GetProperty("schemaVersion").GetString();
        var documentTime = document.RootElement.GetProperty("creationDate").GetDateTime();
        var events = document.RootElement.GetProperty("epcisBody").GetProperty("eventList").EnumerateArray().Select(x => JsonEventParser.Create(x, extensions).Parse()).ToList();

        return new Request
        {
            SchemaVersion = schemaVersion,
            DocumentTime = documentTime,
            Events = events
        };
    }
}
