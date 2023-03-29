using FasTnT.Application.Domain.Exceptions;
using System.Text.Json;

namespace FasTnT.Host.Features.v2_0.Communication.Parsers;

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
            var namespaces = context.Value.EnumerateArray()
                .Where(x => x.ValueKind == JsonValueKind.Object)
                .Select(x => x.EnumerateObject().First());

            foreach (var ctxNamespace in namespaces)
            {
                parsed[ctxNamespace.Name] = ctxNamespace.Value.GetString();
            }
        }

        return new(parsed);
    }

    public static Namespaces ParseHeader(string[] headerContext)
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

    public (string Namespace, string Name) ParseName(string name)
    {
        var parts = name.Split(':', 2);

        if (parts.Length == 1)
        {
            return (string.Empty, name);
        }

        if (!_namespaces.TryGetValue(parts[0], out var namespaceName))
        {
            throw new EpcisException(ExceptionType.ValidationException, $"Namespace not found: {parts[0]}");
        }

        return (namespaceName, parts[1]);
    }
}