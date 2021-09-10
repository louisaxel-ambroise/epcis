using FasTnT.Domain.Commands.Subscribe;
using FasTnT.Domain.Commands.Unsubscribe;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Queries.GetQueryNames;
using FasTnT.Domain.Queries.GetStandardVersion;
using FasTnT.Domain.Queries.GetSubscriptionIds;
using FasTnT.Domain.Queries.GetVendorVersion;
using FasTnT.Domain.Queries.Poll;
using FasTnT.Formatter.Xml.Utils;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatter.Xml.Formatters
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
            var reasonElt = !string.IsNullOrEmpty(reason) ? new XElement(nameof(reason), reason) : default;
            var severityElt = new XElement(nameof(severity), severity);

            return new XElement(type.ToString(), reasonElt, severityElt);
        }

        public static XElement FormatSubscriptionIds(GetSubscriptionIdsResult response)
        {
            var subscriptions = response.SubscriptionIDs.Select(x => new XElement("string", x));

            return new(XName.Get(nameof(GetSubscriptionIdsResult), Namespaces.Query), subscriptions);
        }

        public static XElement FormatGetQueryNames(GetQueryNamesResult response)
        {
            var queryNames = response.QueryNames.Select(x => new XElement("string", x));

            return new (XName.Get(nameof(GetQueryNamesResult), Namespaces.Query), queryNames);
        }

        public static XElement FormatVendorVersion(GetVendorVersionResult response)
        {
            return new XElement(XName.Get(nameof(GetVendorVersionResult), Namespaces.Query), response.Version);
        }

        public static XElement FormatStandardVersion(GetStandardVersionResult response)
        {
            return new XElement(XName.Get(nameof(GetStandardVersionResult), Namespaces.Query), response.Version);
        }

        public static XElement FormatUnsubscribeResponse(UnsubscribeResult _)
        {
            return new(XName.Get(nameof(UnsubscribeResult), Namespaces.Query));
        }

        public static XElement FormatSubscribeResponse(SubscribeResult _)
        {
            return new(XName.Get(nameof(SubscribeResult), Namespaces.Query));
        }
    }
}
