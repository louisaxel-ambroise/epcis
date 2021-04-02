using FasTnT.Application.Commands;
using FasTnT.Application.Queries.GetStandardVersion;
using FasTnT.Application.Queries.Poll;
using FasTnT.Domain.Exceptions;
using FasTnT.Formatter.Xml.Utils;
using System.Collections.Generic;
using System.Xml.Linq;

namespace FasTnT.Formatter.Xml
{
    public static class XmlResponseFormatter
    {
        public static string FormatPoll(PollResponse response)
        {
            var resultName = "EventList";
            var resultList = default(IEnumerable<XElement>);

            if (response.EventList.Count > 0)
            {
                resultName = "EventList";
                resultList = XmlEventFormatter.FormatList(response.EventList);
            }
            else if (response.MasterdataList.Count > 0)
            {
                resultName = "VocabularyList";
                //resultList = XmlMasterdataFormatter.FormatMasterData(response.MasterdataList, cancellationToken);
            }

            var queryResults = new XElement(XName.Get("QueryResults", Namespaces.Query),
                new XElement("queryName", response.QueryName),
                !string.IsNullOrEmpty(response.SubscriptionId) ? new XElement("subscriptionID", response.SubscriptionId) : null,
                new XElement("resultsBody", new XElement(resultName, resultList))
            );

            return SoapResponseBuilder.WrapSoap11(queryResults);
        }

        public static string FormatError(EpcisException exception)
        {
            var reason = !string.IsNullOrEmpty(exception.Message) ? new XElement("reason", exception.Message) : null;
            var severity = (exception.Severity != null) ? new XElement("severity", exception.Severity.DisplayName) : null;

            return new XElement(exception.ExceptionType.DisplayName, reason, severity).ToString();
        }

        public static string FormatVendorVersion(GetVendorVersionResponse response)
        {
            var formatted = new XElement(XName.Get("GetVendorVersionResult", Namespaces.Query), response.Version);

            return SoapResponseBuilder.WrapSoap11(formatted);
        }

        public static string FormatStandardVersion(GetStandardVersionResponse response)
        {
            var formatted = new XElement(XName.Get("GetStandardVersionResult", Namespaces.Query), response.Version);

            return SoapResponseBuilder.WrapSoap11(formatted);
        }

        public static string FormatCaptureResponse(CaptureEpcisRequestResponse _)
        {
            return string.Empty;
        }
    }

    public class SoapResponseBuilder    {
        internal static string WrapSoap11(XElement response)
        {
            return new XDocument(new XElement(XName.Get("Envelope", Namespaces.SoapEnvelop),
                new XAttribute(XNamespace.Xmlns + "soapenv", Namespaces.SoapEnvelop),
                new XAttribute(XNamespace.Xmlns + "epcisq", Namespaces.Query),
                new XElement(XName.Get("Body", Namespaces.SoapEnvelop), response)
            )).ToString(SaveOptions.DisableFormatting);
        }
    }
}
