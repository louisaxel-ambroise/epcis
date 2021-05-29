using FasTnT.Domain.Queries.GetStandardVersion;
using FasTnT.Domain.Queries.Poll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatter.Xml
{
    public static class XmlQueryParser
    {
        public static PollQuery ParsePollQuery(XElement element)
        {
            var queryName = element.Element("queryName").Value;
            var parameters = ParsePollParameters(element.Element("params")?.Elements()).ToArray();

            return new(queryName, parameters);
        }

        public static GetVendorVersionQuery ParseGetVendorVersion() => new();
        public static GetStandardVersionQuery ParseGetStandardVersion() => new();

        internal static IEnumerable<QueryParameter> ParsePollParameters(IEnumerable<XElement> elements)
        {
            foreach (var element in elements ?? Array.Empty<XElement>())
            {
                var name = element.Element("name")?.Value?.Trim();
                var values = element.Element("value")?.HasElements ?? false ? element.Element("value").Elements().Select(x => x.Value) : new[] { element.Element("value")?.Value };

                yield return new(name, values.ToArray());
            }
        }
    }
}
