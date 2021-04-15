using FasTnT.Formatter.Xml;
using FasTnT.Formatter.Xml.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FasTnT.Host.Extensions
{
    public static class SoapExtensions
    {
        private static readonly XAttribute[] CommonAttributes = { new(XNamespace.Xmlns + "soapenv", Namespaces.SoapEnvelop), new(XNamespace.Xmlns + "epcisq", Namespaces.Query) };

        public static async Task FormatSoap(this HttpResponse response, XElement element, CancellationToken cancellationToken)
        {
            var envelope = new XDocument(
                new XElement(XName.Get("Envelope", Namespaces.SoapEnvelop), CommonAttributes,
                new XElement(XName.Get("Body", Namespaces.SoapEnvelop), element)
            ));

            using var xmlWriter = XmlWriter.Create(response.Body, new XmlWriterSettings { Async = true });

            await envelope.WriteToAsync(xmlWriter, cancellationToken);
        }

        public static async Task<object> ParseSoapEnvelope(this HttpRequest request, CancellationToken cancellationToken)
        {
            var document = await XDocument.LoadAsync(request.Body, LoadOptions.None, cancellationToken);
            var envelopBody = document.Element(XName.Get("Envelope", Namespaces.SoapEnvelop))?.Element(XName.Get("Body", Namespaces.SoapEnvelop));

            if (envelopBody == null || !envelopBody.HasElements)
            {
                return null;
            }

            var queryElement = envelopBody.Elements().SingleOrDefault(x => x.Name.NamespaceName == Namespaces.Query);

            return queryElement.Name.LocalName switch
            {
                "Poll" => XmlQueryParser.ParsePollQuery(queryElement),
                "GetVendorVersion" => XmlQueryParser.ParseGetVendorVersion(),
                "GetStandardVersion" => XmlQueryParser.ParseGetStandardVersion(),
                _ => throw new Exception()
            };
        }
    }
}
