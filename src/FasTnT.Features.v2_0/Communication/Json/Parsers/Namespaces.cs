using System.Text.Json;

namespace FasTnT.Features.v2_0.Communication.Json.Parsers;

public class Namespaces
{
    private readonly Dictionary<string, string> _namespaces;

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

        foreach (var header in headerContext.Select(x => x.Split('=', 2)).Where(x => x.Length == 2))
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