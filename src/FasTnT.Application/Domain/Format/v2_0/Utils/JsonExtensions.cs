namespace FasTnT.Application.Domain.Format.v2_0.Utils;

public static class JsonExtensions
{
    public static void AddIfNotNull(this IDictionary<string, object> dictionary, string value, string key)
    {
        if (value is null)
        {
            return;
        }

        dictionary[key] = value;
    }

    public static string ToUpperString(this object value) => value.ToString().ToUpperInvariant();

    // Builds a context for JSON format.
    // key=namespace, value=prefix
    // The prefixes are incremental (ext1, ext2, ext...)
    public static IDictionary<string, string> BuildContext(this IEnumerable<string> namespaces)
    {
        return namespaces
            .Select((x, i) => (Value: x, Index: i))
            .ToDictionary(x => x.Value, x => $"ext{x.Index}");
    }
}
