using FasTnT.Host.Communication.Json.Formatters;

namespace FasTnT.Host.Subscriptions.Formatters;

public sealed class JsonSubscriptionFormatter : ISubscriptionFormatter
{
    public static ISubscriptionFormatter Instance => new JsonSubscriptionFormatter();
    
    public string ContentType => "application/json";
    public string Format<T>(T result) => JsonResponseFormatter.Format(result);
}
