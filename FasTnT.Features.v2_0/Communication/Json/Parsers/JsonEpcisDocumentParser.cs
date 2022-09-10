using FasTnT.Domain.Model;
using System.Text.Json;

namespace FasTnT.Formatter.v2_0.Json;

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

public class Namespaces
{
    private Dictionary<string, string> _namespaces;

    public Namespaces(Dictionary<string, string> namespaces)
    {
        _namespaces = namespaces ?? new();
    }

    public static Namespaces Parse(JsonElement? context)
    {
        var parsed = new Dictionary<string, string>();

        if (context != null && context.Value.ValueKind == JsonValueKind.Array)
        {
            foreach (var ctxNamespace in context.Value.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.Object).Select(x => x.EnumerateObject().First()))
            {
                parsed[ctxNamespace.Name] = ctxNamespace.Value.GetString();
            }
        }

        return new(parsed);
    }

    internal static Namespaces ParseHeader(string[] headerContext)
    {
        var parsed = new Dictionary<string, string>();

        foreach (var header in headerContext.Select(x => x.Split('=', 2)))
        {
            parsed[header[0]] = header[1];
        }

        return new(parsed);
    }

    public Namespaces Merge(Namespaces other)
    {
        foreach (var name in other._namespaces)
        {
            _namespaces[name.Key] = name.Value;
        }

        return this;
    }

    public string this[string key] => _namespaces[key];
}