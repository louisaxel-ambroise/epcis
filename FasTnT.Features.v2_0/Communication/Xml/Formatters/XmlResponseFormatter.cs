using FasTnT.Domain.Commands.Subscribe;
using FasTnT.Domain.Commands.Unsubscribe;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Queries;
using FasTnT.Formatter.Xml.Utils;

namespace FasTnT.Formatter.Xml.Formatters;
 
public static class XmlResponseFormatter
{
    public static string Format(IEpcisResponse response)
    {
        var element = response switch
        {
            PollResponse poll => FormatPoll(poll),
            GetSubscriptionIdsResult subscriptionIds => FormatSubscriptionIds(subscriptionIds),
            GetQueryNamesResult queryNames => FormatGetQueryNames(queryNames),
            GetVendorVersionResult vendorVersion => FormatVendorVersion(vendorVersion),
            GetStandardVersionResult standardVersion => FormatStandardVersion(standardVersion),
            UnsubscribeResult unsubscription => FormatUnsubscribeResponse(unsubscription),
            SubscribeResult subscription => FormatSubscribeResponse(subscription),
            _ => FormatError(EpcisException.Default)
        };

        return element.ToString(SaveOptions.OmitDuplicateNamespaces | SaveOptions.DisableFormatting);
    }

    public static XElement FormatPoll(PollResponse response)
    {
        var (resultName, resultList) = response switch
        {
            PollEventResponse => (nameof(response.EventList), XmlEventFormatter.FormatList(response.EventList)),
            PollMasterdataResponse => (nameof(response.VocabularyList), XmlMasterdataFormatter.FormatMasterData(response.VocabularyList)),
            _ => throw new NotImplementedException()
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
        var reason = !string.IsNullOrEmpty(exception.Message) ? new XElement("reason", exception.Message) : default;
        var severity = exception.Severity != ExceptionSeverity.None ? new XElement("severity", exception.Severity.ToString()) : default;
        var queryName = !string.IsNullOrEmpty(exception.QueryName) ? new XElement("queryName", exception.QueryName) : default;
        var subscriptionId = !string.IsNullOrEmpty(exception.SubscriptionId) ? new XElement("subscriptionID", exception.SubscriptionId) : default;

        return new(XName.Get(exception.ExceptionType.ToString(), Namespaces.Query), reason, severity, queryName, subscriptionId);
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
