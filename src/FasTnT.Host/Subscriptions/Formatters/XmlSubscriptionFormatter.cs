using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Queries;
using FasTnT.Host.Features.v1_2.Communication.Formatters;
using FasTnT.Host.Features.v1_2.Communication.Utils;

namespace FasTnT.Host.Subscriptions.Formatters;

public class XmlSubscriptionFormatter : ISubscriptionFormatter
{
    public static readonly ISubscriptionFormatter Instance = new XmlSubscriptionFormatter();

    public string Name => nameof(XmlSubscriptionFormatter);
    public string ContentType => "application/text+xml";

    private XmlSubscriptionFormatter() { }

    public string FormatResult(string name, QueryResponse response)
    {
        var rootName = XName.Get("EPCISQueryDocument", "urn:epcglobal:epcis-query:xsd:1");

        var content = new XElement(XName.Get("QueryResults", Namespaces.Query),
            new XElement("queryName", response.QueryName),
            new XElement("subscriptionID", name),
            new XElement("resultsBody", new XElement("EventList", XmlEventFormatter.FormatList(response.EventList))));
        var attributes = new[] { new XAttribute("creationDate", DateTime.UtcNow), new XAttribute("schemaVersion", "1.2") };

        return new XElement(rootName, attributes, new XElement("EPCISBody", content)).ToString(SaveOptions.DisableFormatting);
    }

    public string FormatError(string name, string query, EpcisException error)
    {
        var reason = new XElement("reason", error.Message);
        var severity = new XElement("severity", error.Severity.ToString());
        var queryName = new XElement("queryName", query);
        var subscriptionId = new XElement("subscriptionID", name);
        var elementName = XName.Get(error.ExceptionType.ToString(), Namespaces.Query);

        return new XElement(elementName, reason, severity, queryName, subscriptionId)
            .ToString(SaveOptions.DisableFormatting);
    }
}
