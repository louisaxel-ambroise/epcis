using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Features.v1_2.Communication.Utils;
using FasTnT.Features.v1_2.Endpoints.Interfaces.Queries;

namespace FasTnT.Features.v1_2.Communication.Formatters;

public static class XmlResponseFormatter
{
    public static XElement Format(object response)
    {
        return response switch
        {
            PollResult poll => FormatPoll(poll),
            GetSubscriptionIDsResult subscriptionIds => FormatSubscriptionIds(subscriptionIds),
            GetQueryNamesResult queryNames => FormatGetQueryNames(queryNames),
            GetVendorVersionResult vendorVersion => FormatVendorVersion(vendorVersion),
            GetStandardVersionResult standardVersion => FormatStandardVersion(standardVersion),
            // TODO: subscriptions
            //UnsubscribeResult unsubscription => FormatUnsubscribeResponse(unsubscription),
            //SubscribeResult subscription => FormatSubscribeResponse(subscription),
            _ => FormatError(EpcisException.Default)
        };
    }

    public static XElement FormatPoll(PollResult response)
    {
        var (resultName, resultList) = response switch
        {
            _ when response.EventList is not null => (nameof(response.EventList), XmlEventFormatter.FormatList(response.EventList)),
            _ when response.VocabularyList is not null => (nameof(response.VocabularyList), XmlMasterdataFormatter.FormatMasterData(response.VocabularyList)),
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

    public static XElement FormatSubscriptionIds(GetSubscriptionIDsResult response)
    {
        var subscriptions = response.SubscriptionIDs.Select(x => new XElement("string", x));

        return new(XName.Get(nameof(GetSubscriptionIDsResult), Namespaces.Query), subscriptions);
    }

    public static XElement FormatGetQueryNames(GetQueryNamesResult response)
    {
        var queryNames = response.QueryNames.Select(x => new XElement("string", x));

        return new(XName.Get(nameof(GetQueryNamesResult), Namespaces.Query), queryNames);
    }

    public static XElement FormatVendorVersion(GetVendorVersionResult response)
    {
        return new XElement(XName.Get(nameof(GetVendorVersionResult), Namespaces.Query), response.Version);
    }

    public static XElement FormatStandardVersion(GetStandardVersionResult response)
    {
        return new XElement(XName.Get(nameof(GetStandardVersionResult), Namespaces.Query), response.Version);
    }

    public static XElement FormatUnsubscribeResponse()
    {
        return new(XName.Get("UnsubscribeResult", Namespaces.Query));
    }

    public static XElement FormatSubscribeResponse()
    {
        return new(XName.Get("SubscribeResult", Namespaces.Query));
    }
}
