namespace FasTnT.Formatter.v2_0.Utils;

public static class Extensions
{
    public static void AddIfNotNull(this IDictionary<string, object> dictionary, string value, string key)
    {
        if (value is null) return;

        dictionary[key] = value;
    }
}
