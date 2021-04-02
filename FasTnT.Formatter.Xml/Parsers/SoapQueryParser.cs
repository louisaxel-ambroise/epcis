using FasTnT.Application.Queries.GetStandardVersion;
using FasTnT.Application.Queries.Poll;
using FasTnT.Formatter.Xml.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FasTnT.Formatter.Xml
{
    public class SoapQueryParser
    {
        private readonly XElement _queryElement;

        public string Action { get; private set; }

        public SoapQueryParser(XElement queryElement, string action)
        {
            _queryElement = queryElement;
            Action = action;
        }

        public PollQuery ParsePollQuery()
        {
            var queryName = _queryElement.Element("queryName").Value;
            var parameters = ParsePollParameters(_queryElement.Element("params")?.Elements()).ToArray();

            return new PollQuery
            {
                QueryName = queryName,
                Parameters = parameters
            };
        }

        public GetVendorVersionQuery ParseGetVendorVersion() => new GetVendorVersionQuery();
        public GetStandardVersionQuery ParseGetStandardVersion() => new GetStandardVersionQuery();

        internal static IEnumerable<QueryParameter> ParsePollParameters(IEnumerable<XElement> elements)
        {
            foreach (var element in elements ?? Array.Empty<XElement>())
            {
                var name = element.Element("name")?.Value?.Trim();
                var values = element.Element("value")?.HasElements ?? false ? element.Element("value").Elements().Select(x => x.Value) : new[] { element.Element("value")?.Value };

                yield return new QueryParameter
                {
                    Name = name,
                    Values = values.ToArray()
                };
            }
        }

        public static async Task<SoapQueryParser> ParseEnvelop(Stream stream, CancellationToken cancellationToken)
        {
            var document = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);
            var envelopBody = document.Element(XName.Get("Envelope", Namespaces.SoapEnvelop))?.Element(XName.Get("Body", Namespaces.SoapEnvelop));

            if (envelopBody == null || !envelopBody.HasElements)
            {
                throw new Exception("Malformed SOAP request");
            }
            else
            {
                var action = envelopBody.Elements().SingleOrDefault(x => x.Name.NamespaceName == Namespaces.Query).Name.LocalName;
                return new SoapQueryParser(envelopBody.Elements().Single(), action);
            }
        }
    }
}
