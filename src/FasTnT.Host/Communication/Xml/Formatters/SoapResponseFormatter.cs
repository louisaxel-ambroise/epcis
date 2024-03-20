using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Host.Communication.Xml.Utils;
using FasTnT.Host.Endpoints.Interfaces;

namespace FasTnT.Host.Communication.Xml.Formatters;

public static class SoapResponseFormatter
{
    public static XElement Format(object response)
    {
        return response switch
        {
            QueryResult poll => FormatQueryResult(poll),
            ListSubscriptionsResult subscriptionIds => FormatSubscriptions(subscriptionIds),
            ListCustomQueriesResult queries => FormatQueries(queries),
            GetVendorVersionResult vendorVersion => FormatVendorVersion(vendorVersion),
            GetStandardVersionResult standardVersion => FormatStandardVersion(standardVersion),
            UnsubscribeResult _ => FormatUnsubscribeResponse(),
            Subscription _ => FormatSubscribeResponse(),
            _ => FormatError(EpcisException.Default)
        };
    }

    public static XElement FormatQueryResult(QueryResult response)
    {
        var (resultName, resultList) = response switch
        {
            _ when response.Response.EventList is not null => (nameof(response.Response.EventList), XmlEventFormatter.FormatList(response.Response.EventList)),
            _ when response.Response.VocabularyList is not null => (nameof(response.Response.VocabularyList), XmlMasterdataFormatter.FormatMasterData(response.Response.VocabularyList)),
            _ => throw new NotImplementedException()
        };

        var queryResults = new XElement(XName.Get("QueryResults", Namespaces.Query),
            new XElement("queryName", response.Response.QueryName),
            !string.IsNullOrEmpty(response.Response.SubscriptionName) ? new XElement("subscriptionID", response.Response.SubscriptionName) : null,
            new XElement("resultsBody", new XElement(resultName, resultList))
        );

        if (response.Response.EventList?.Count > 0)
        {
            var customNamespaces = response.Response.EventList.SelectMany(x => x.Fields.Select(x => x.Namespace)).Where(IsCustomNamespace).Distinct().ToArray();
            
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

    public static XElement FormatSubscriptions(ListSubscriptionsResult response)
    {
        var subscriptions = response.Subscriptions.Select(x => new XElement("string", x.Name));

        return new(XName.Get("GetSubscriptionIDsResult", Namespaces.Query), subscriptions);
    }

    public static XElement FormatQueries(ListCustomQueriesResult response)
    {
        var queryNames = response.Queries.Select(x => new XElement("string", x.Name));

        return new(XName.Get("GetQueryNamesResult", Namespaces.Query), queryNames);
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