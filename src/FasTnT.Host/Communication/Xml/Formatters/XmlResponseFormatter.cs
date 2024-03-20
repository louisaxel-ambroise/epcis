using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Queries;
using FasTnT.Host.Communication.Xml.Utils;
using FasTnT.Host.Endpoints.Interfaces;

namespace FasTnT.Host.Communication.Xml.Formatters;

public static class XmlResponseFormatter
{
    public static string Format<T>(T response)
    {
        var element = response is QueryResult poll
            ? FormatPoll(poll.Response)
            : throw new FormatException();

        return element.ToString(SaveOptions.OmitDuplicateNamespaces | SaveOptions.DisableFormatting);
    }

    public static XElement FormatPoll(QueryResponse response)
    {
        var (resultName, resultList) = response switch
        {
            _ when response.EventList is not null => (nameof(response.EventList), XmlV2EventFormatter.Instance.FormatList(response.EventList)),
            _ when response.VocabularyList is not null => (nameof(response.VocabularyList), XmlMasterdataFormatter.FormatMasterData(response.VocabularyList)),
            _ => throw new NotImplementedException()
        };

        var queryResults = new XElement(XName.Get("QueryResults", Namespaces.Query),
            new XAttribute(XNamespace.Xmlns + "epcisq", Namespaces.Query),
            new XAttribute(XNamespace.Xmlns + "xsd", Namespaces.XSD),
            new XAttribute(XNamespace.Xmlns + "xsi", Namespaces.XSI),
            new XElement("queryName", response.QueryName),
            new XElement("resultsBody", new XElement(resultName, resultList))
        );

        if (response is QueryResponse pollResponse && pollResponse.EventList?.Count > 0)
        {
            var customNamespaces = pollResponse.EventList.SelectMany(x => x.Fields.Select(x => x.Namespace)).Where(IsCustomNamespace).Distinct().ToArray();

            for (var i = 0; i < customNamespaces.Length; i++)
            {
                queryResults.Add(new XAttribute(XNamespace.Xmlns + $"ext{i + 1}", customNamespaces[i]));
            }
        }

        return queryResults;
    }

    private static bool IsCustomNamespace(string uri)
    {
        return !string.IsNullOrWhiteSpace(uri) && !Namespaces.ContainsUri(uri);
    }

    public static XElement FormatError(EpcisException error)
    {
        var type = new XElement(XName.Get("type", "urn:ietf:rfc:7807"), $"epcisException:{error.ExceptionType}");
        var title = new XElement(XName.Get("title", "urn:ietf:rfc:7807"), error.Message);
        var status = new XElement(XName.Get("status", "urn:ietf:rfc:7807"), GetHttpStatusCode(error));

        return new(XName.Get("problem", "urn:ietf:rfc:7807"), type, title, status);
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
}
