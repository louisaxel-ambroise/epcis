using FasTnT.Domain.Exceptions;
using FasTnT.Formatter.Xml;
using FasTnT.Formatter.Xml.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace FasTnT.Host.Extensions
{
    public static class SoapExtensions
    {
        public static async Task FormatSoap(this HttpResponse response, XElement element, CancellationToken cancellationToken)
        {
            response.ContentType = "application/xml";

            var body = new XElement(XName.Get("Body", Namespaces.SoapEnvelop), element);
            var envelope = new XElement(XName.Get("Envelope", Namespaces.SoapEnvelop), body);
            var xmlResponse = new XDocument(envelope);

            envelope.Add(new XAttribute(XNamespace.Xmlns + "soapenv", Namespaces.SoapEnvelop), new XAttribute(XNamespace.Xmlns + "epcisq", Namespaces.Query));

            using var xmlWriter = XmlWriter.Create(response.Body, new XmlWriterSettings { Async = true, NamespaceHandling = NamespaceHandling.OmitDuplicates });

            await xmlResponse.WriteToAsync(xmlWriter, cancellationToken);
        }

        public static async Task<object> ParseSoapEnvelope(this HttpRequest request, CancellationToken cancellationToken)
        {
            var document = await XDocument.LoadAsync(request.Body, LoadOptions.None, cancellationToken);
            var envelopBody = document.XPathSelectElement("SoapEnvelop:Envelope/SoapEnvelop:Body", Namespaces.Resolver);

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
                "GetQueryNames" => XmlQueryParser.ParseGetQueryNames(),
                "Subscribe" => XmlQueryParser.ParseSubscribe(queryElement),
                "Unsubscribe" => XmlQueryParser.ParseUnsubscribe(queryElement),
                "GetSubscriptionIDs" => XmlQueryParser.ParseGetSubscriptionIds(queryElement),
                _ => throw new EpcisException(ExceptionType.ValidationException, "Unknown Query element")
            };
        }
    }
}
