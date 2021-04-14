using FasTnT.Application.Commands;
using FasTnT.Application.Queries.GetStandardVersion;
using FasTnT.Application.Queries.Poll;
using FasTnT.Domain.Exceptions;
using FasTnT.Formatter.Xml.Utils;
using System.Xml.Linq;

namespace FasTnT.Formatter.Xml
{
    public static class XmlResponseFormatter
    {
        public static XElement FormatPoll(PollResponse response)
        {
            var (resultName, resultList) = response.MasterdataList.Count switch
            {
                > 0  => ("VocabularyList", default), //XmlMasterdataFormatter.FormatMasterData(response.MasterdataList);
                <= 0 => ("EventList", XmlEventFormatter.FormatList(response.EventList))
            };

            var queryResults = new XElement(XName.Get("QueryResults", Namespaces.Query),
                new XElement("queryName", response.QueryName),
                !string.IsNullOrEmpty(response.SubscriptionId) ? new XElement("subscriptionID", response.SubscriptionId) : null,
                new XElement("resultsBody", new XElement(resultName, resultList))
            );

            return queryResults;
        }

        public static XElement FormatError(EpcisException exception)
        {
            var reason = !string.IsNullOrEmpty(exception.Message) ? new XElement("reason", exception.Message) : null;
            var severity = (exception.Severity != null) ? new XElement("severity", exception.Severity.DisplayName) : null;

            return new XElement(exception.ExceptionType.DisplayName, reason, severity);
        }

        public static XElement FormatVendorVersion(GetVendorVersionResponse response)
        {
            return new XElement(XName.Get("GetVendorVersionResult", Namespaces.Query), response.Version);
        }

        public static XElement FormatStandardVersion(GetStandardVersionResponse response)
        {
            return new XElement(XName.Get("GetStandardVersionResult", Namespaces.Query), response.Version);
        }

        public static XElement FormatCaptureResponse(CaptureEpcisRequestResponse _) => default;
    }
}
