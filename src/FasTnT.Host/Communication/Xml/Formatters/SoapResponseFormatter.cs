using FasTnT.Domain.Exceptions;
using FasTnT.Host.Communication.Xml.Utils;
using FasTnT.Host.Features.v1_2.Endpoints.Interfaces;

namespace FasTnT.Host.Communication.Xml.Formatters;

public static class SoapResponseFormatter
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
            UnsubscribeResult _ => FormatUnsubscribeResponse(),
            SubscribeResult _ => FormatSubscribeResponse(),
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

        if (response.EventList?.Count > 0)
        {
            var customNamespaces = response.EventList.SelectMany(x => x.Fields.Select(x => x.Namespace)).Where(IsCustomNamespace).Distinct().ToArray();

            for (var i = 0; i < customNamespaces.Length; i++)
            {
                queryResults.Add(new XAttribute(XNamespace.Xmlns + $"ext{i + 1}", customNamespaces[i]));
            }
        }

        return queryResults;
    }

    public static int GetHttpStatusCode(EpcisException error)
    {
        return error.ExceptionType switch
        {
            ExceptionType.NoSuchNameException => 404,
            ExceptionType.NoSuchSubscriptionException => 404,
            ExceptionType.QueryTooComplexException => 413,
            ExceptionType.QueryTooLargeException => 413,
            ExceptionType.CaptureLimitExceededException => 413,
            ExceptionType.ImplementationException => 500,
            _ => 400
        };
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

    private static bool IsCustomNamespace(string uri)
    {
        return !string.IsNullOrWhiteSpace(uri)
            && XNamespace.Xmlns != uri
            && !Namespaces.ContainsUri(uri);
    }
}