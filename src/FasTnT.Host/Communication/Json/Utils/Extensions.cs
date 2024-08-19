namespace FasTnT.Host.Communication.Json.Utils;

public static class Extensions
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
    public static Dictionary<string, string> BuildContext(this IEnumerable<string> namespaces)
    {
        return namespaces
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct()
            .Select((x, i) => (Value: x, Index: i))
            .ToDictionary(x => x.Value, x => $"ext{x.Index}");
    }
}
