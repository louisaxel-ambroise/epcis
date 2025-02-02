namespace FasTnT.Host.Subscriptions.Formatters;

public interface ISubscriptionFormatter
{
    public string ContentType { get; }

    string Format<T>(T result);
}
