using FasTnT.Domain.Commands;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Queries.GetStandardVersion;
using FasTnT.Domain.Queries.Poll;
using FasTnT.Formatter.Xml.Utils;
using System.Xml.Linq;

namespace FasTnT.Formatter.Xml
{
    public static class XmlResponseFormatter
    {
        public static XElement FormatPoll(PollResponse response)
        {
            var (resultName, resultList) = response.VocabularyList != default 
                ? (nameof(response.VocabularyList), XmlMasterdataFormatter.FormatMasterData(response.VocabularyList)) 
                : (nameof(response.EventList), XmlEventFormatter.FormatList(response.EventList));

            var queryResults = new XElement(XName.Get("QueryResults", Namespaces.Query),
                new XElement("queryName", response.QueryName),
                !string.IsNullOrEmpty(response.SubscriptionId) ? new XElement("subscriptionID", response.SubscriptionId) : null,
                new XElement("resultsBody", new XElement(resultName, resultList))
            );

            return queryResults;
        }

        public static XElement FormatUnexpectedError()
        {
            return FormatError(ExceptionType.ImplementationException, "An unexpected error occured", ExceptionSeverity.Error);
        }

        public static XElement FormatError(EpcisException exception)
        {
            return FormatError(exception.ExceptionType, exception.Message, exception.Severity);
        }

        private static XElement FormatError(ExceptionType type, string reason, ExceptionSeverity severity)
        {
            var reasonElt = !string.IsNullOrEmpty(reason) ? new XElement("reason", reason) : default;
            var severityElt = !string.IsNullOrEmpty(reason) ? new XElement("severity", severity) : default;

            return new XElement(type.DisplayName, reasonElt, severityElt);
        }

        public static XElement FormatVendorVersion(GetVendorVersionResult response)
        {
            return new XElement(XName.Get(nameof(GetVendorVersionResult), Namespaces.Query), response.Version);
        }

        public static XElement FormatStandardVersion(GetStandardVersionResult response)
        {
            return new XElement(XName.Get(nameof(GetStandardVersionResult), Namespaces.Query), response.Version);
        }

        public static XElement FormatCaptureResponse(CaptureEpcisRequestResponse _) => default;
    }
}
