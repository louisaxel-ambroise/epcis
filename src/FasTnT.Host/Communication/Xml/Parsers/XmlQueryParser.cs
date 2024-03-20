using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Host.Endpoints.Interfaces;
using FasTnT.Host.Endpoints.Responses.Soap;
using FasTnT.Host.Subscriptions.Formatters;

namespace FasTnT.Host.Communication.Xml.Parsers;

public static class SoapQueryParser
{
    public static SoapEnvelope Parse(XElement queryElement)
    {
        return queryElement?.Name?.LocalName switch
        {
            "Poll" => ParsePollQuery(queryElement),
            "Subscribe" => ParseSubscribe(queryElement),
            "Unsubscribe" => ParseUnsubscribe(queryElement),
            "GetSubscriptionIDs" => ParseGetSubscriptionIds(queryElement),
            var other => new SoapEnvelope(other, [], null)
        };
    }

    public static SoapEnvelope ParsePollQuery(XElement element)
    {
        var queryName = element.Element("queryName").Value;
        var parameters = ParseQueryParameters(element.Element("params")?.Elements()).ToArray();

        return new("Poll", new() { { "queryName", queryName } }, new QueryContext(parameters));
    }

    public static SoapEnvelope ParseUnsubscribe(XElement element)
    {
        return new SoapEnvelope("Unsubscribe", [], new Unsubscribe(element.Element("subscriptionID").Value));
    }

    public static SoapEnvelope ParseSubscribe(XElement element)
    {
        var subscription = new Subscription
        {
            Name = element.Element("subscriptionID").Value,
            QueryName = element.Element("queryName").Value,
            Destination = element.Element("dest").Value,
            FormatterName = nameof(XmlSubscriptionFormatter),
            Trigger = element.Element("controls")?.Element("trigger")?.Value,
            ReportIfEmpty = bool.Parse(element.Element("controls").Element("reportIfEmpty").Value),
            InitialRecordTime = DateTime.TryParse(element.Element("controls")?.Element("initialRecordTime")?.Value ?? string.Empty, null, DateTimeStyles.AdjustToUniversal, out DateTime date) ? date : DateTime.UtcNow,
            Parameters = ParseQueryParameters(element.Element("params")?.Elements()).ToList(),
            Schedule = ParseQuerySchedule(element.Element("controls")?.Element("schedule"))
        };
        subscription.LastExecutedTime = subscription.InitialRecordTime;

        var subscriptionRequest = new SubscriptionRequest(subscription.QueryName, subscription);

        return new SoapEnvelope("Subscribe", [], subscriptionRequest);
    }

    public static SoapEnvelope ParseGetSubscriptionIds(XElement element)
    {
        return new("GetSubscriptionIDs", [], new ListSubscriptionsRequest(element.Element("queryName")?.Value));
    }

    private static IEnumerable<QueryParameter> ParseQueryParameters(IEnumerable<XElement> elements)
    {
        foreach (var element in elements ?? Array.Empty<XElement>())
        {
            var name = element.Element("name")?.Value?.Trim();
            var values = element.Element("value").HasElements
                ? element.Element("value").Elements().Select(x => x.Value)
                : new[] { element.Element("value").Value };

            yield return QueryParameter.Create(name, values.ToArray());
        }
    }

    private static SubscriptionSchedule ParseQuerySchedule(XElement element)
    {
        if (element == null || element.IsEmpty)
        {
            return default;
        }

        return new()
        {
            Second = element.Element("second")?.Value ?? string.Empty,
            Minute = element.Element("minute")?.Value ?? string.Empty,
            Hour = element.Element("hour")?.Value ?? string.Empty,
            Month = element.Element("month")?.Value ?? string.Empty,
            DayOfMonth = element.Element("dayOfMonth")?.Value ?? string.Empty,
            DayOfWeek = element.Element("dayOfWeek")?.Value ?? string.Empty
        };
    }
}