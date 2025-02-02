using FasTnT.Host.Communication.Xml.Formatters;

namespace FasTnT.Host.Subscriptions.Formatters;

public class XmlSubscriptionFormatter : ISubscriptionFormatter
{
    public static ISubscriptionFormatter Instance => new XmlSubscriptionFormatter();
    
    public string ContentType => "application/text+xml";
    public string Format<T>(T result) => XmlResponseFormatter.Format(result);
}
