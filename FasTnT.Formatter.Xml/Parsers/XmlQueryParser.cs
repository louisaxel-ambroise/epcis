using FasTnT.Domain.Commands.Subscribe;
using FasTnT.Domain.Commands.Unsubscribe;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Queries.GetQueryNames;
using FasTnT.Domain.Queries.GetStandardVersion;
using FasTnT.Domain.Queries.GetSubscriptionIds;
using FasTnT.Domain.Queries.GetVendorVersion;
using FasTnT.Domain.Queries.Poll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatter.Xml.Parsers
{
    public static class XmlQueryParser
    {
        public static object Parse(XElement queryElement)
        {
            return queryElement?.Name?.LocalName switch
            {
                "Poll" => ParsePollQuery(queryElement),
                "GetVendorVersion" => ParseGetVendorVersion(),
                "GetStandardVersion" => ParseGetStandardVersion(),
                "GetQueryNames" => ParseGetQueryNames(),
                "Subscribe" => ParseSubscribe(queryElement),
                "Unsubscribe" => ParseUnsubscribe(queryElement),
                "GetSubscriptionIDs" => ParseGetSubscriptionIds(queryElement),
                _ => throw new EpcisException(ExceptionType.ValidationException, "Unknown Query element")
            };
        }

        public static PollQuery ParsePollQuery(XElement element)
        {
            var queryName = element.Element("queryName").Value;
            var parameters = ParseQueryParameters(element.Element("params")?.Elements()).ToArray();

            return new(queryName, parameters);
        }

        public static UnsubscribeCommand ParseUnsubscribe(XElement element)
        {
            return new()
            {
                SubscriptionId = element.Element("subscriptionID").Value
            };
        }

        public static SubscribeCommand ParseSubscribe(XElement element)
        {
            return new()
            {
                SubscriptionId = element.Element("subscriptionID").Value,
                QueryName = element.Element("queryName").Value,
                Destination = element.Element("dest").Value,
                Trigger = element.Element("controls")?.Element("trigger")?.Value,
                ReportIfEmpty = bool.Parse(element.Element("controls").Element("reportIfEmpty").Value),
                InitialRecordTime = DateTime.TryParse(element.Element("controls")?.Element("initialRecordTime")?.Value ?? string.Empty, out DateTime date) ? date : default(DateTime?),
                Parameters = ParseQueryParameters(element.Element("params")?.Elements()).ToList(),
                Schedule = ParseQuerySchedule(element.Element("controls")?.Element("schedule"))
            };
        }

        public static GetVendorVersionQuery ParseGetVendorVersion() => new();
        public static GetStandardVersionQuery ParseGetStandardVersion() => new();
        public static GetQueryNamesQuery ParseGetQueryNames() => new();

        private static IEnumerable<QueryParameter> ParseQueryParameters(IEnumerable<XElement> elements)
        {
            foreach (var element in elements ?? Array.Empty<XElement>())
            {
                var name = element.Element("name")?.Value?.Trim();
                var values = element.Element("value").HasElements
                    ? element.Element("value").Elements().Select(x => x.Value)
                    : new[] { element.Element("value").Value };

                yield return new(name, values.ToArray());
            }
        }

        public static GetSubscriptionIdsQuery ParseGetSubscriptionIds(XElement element)
        {
            return new()
            {
                QueryName = element.Element("queryName")?.Value
            };
        }

        private static QuerySchedule ParseQuerySchedule(XElement element)
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
}
