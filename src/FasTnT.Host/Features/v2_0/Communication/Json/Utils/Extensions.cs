namespace FasTnT.Host.Features.v2_0.Communication.Json.Utils;

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
}
