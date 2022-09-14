using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Queries;
using FasTnT.Features.v2_0.Communication.Xml.Utils;

namespace FasTnT.Features.v2_0.Communication.Xml.Formatters;

public static class XmlResponseFormatter
{
    public static string Format<T>(T response)
    {
        var element = response is QueryResponse poll
            ? FormatPoll(poll)
            : throw new FormatException();

        return element.ToString(SaveOptions.OmitDuplicateNamespaces | SaveOptions.DisableFormatting);
    }

    public static XElement FormatPoll(QueryResponse response)
    {
        var (resultName, resultList) = response switch
        {
            _ when response.EventList is not null => (nameof(response.EventList), XmlEventFormatter.FormatList(response.EventList)),
            _ when response.VocabularyList is not null => (nameof(response.VocabularyList), XmlMasterdataFormatter.FormatMasterData(response.VocabularyList)),
            _ => throw new NotImplementedException()
        };

        var queryResults = new XElement(XName.Get("QueryResults", Namespaces.Query),
            new XAttribute(XNamespace.Xmlns + "epcisq", Namespaces.Query),
            new XElement("queryName", response.QueryName),
            !string.IsNullOrEmpty(response.SubscriptionId) ? new XElement("subscriptionID", response.SubscriptionId) : null,
            new XElement("resultsBody", new XElement(resultName, resultList))
        );

        // TODO: improve.
        if (response is QueryResponse pollResponse)
        {
            var customNamespaces = pollResponse.EventList.SelectMany(x => x.CustomFields.Select(x => x.Namespace)).Distinct().ToArray();

            for (var i = 0; i < customNamespaces.Length; i++)
            {
                queryResults.Add(new XAttribute(XNamespace.Xmlns + $"ext{i}", customNamespaces[i]));
            }
        }

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
}
