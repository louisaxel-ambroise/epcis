using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Queries.Poll;
using FasTnT.Features.v2_0.Endpoints.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FasTnT.Features.v2_0.Communication.Json.Formatters;

public static class JsonResponseFormatter
{
    private static readonly JsonSerializerOptions Options = new () { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

    public static string Format<T>(T response)
    {
        return response switch {
            CustomQueryDefinitionResult customQuery => FormatCustomQuery(customQuery),
            ListCustomQueriesResult listQueries => FormatListQueries(listQueries),
            QueryResponse poll => FormatPoll(poll),
            _ => FormatError(EpcisException.Default)
        };
    }

    public static string FormatError(EpcisException error)
    {
        return JsonSerializer.Serialize(new
        {
            Type = $"epcisException:{error.ExceptionType}",
            Title = error.Message,
            Status = 400
        });
    }

    private static string FormatCustomQuery(CustomQueryDefinitionResult result)
    {
        var query = new Dictionary<string, object>
        {
            ["name"] = result.Name,
            ["query"] = result.Parameters.Select(x => new Dictionary<string, object> { [x.Name] = x.Values })
        };

        return JsonSerializer.Serialize(query, Options);
    }

    private static string FormatListQueries(ListCustomQueriesResult result)
    {
        var queries = result.Queries.Select(q => new Dictionary<string, object>
        {
            ["name"] = q.Name,
            ["query"] = q.Parameters.Select(x => new Dictionary<string, object> { [x.Name] = x.Values })
        });

        return JsonSerializer.Serialize(queries, Options);
    }

    private static string FormatPoll(QueryResponse result)
    {
        var context = BuildContext(result.EventList.SelectMany(x => x.CustomFields).Select(x => x.Namespace).Distinct());
        var document = new Dictionary<string, object>
        {
            ["@context"] = context.Select(x => (object)new Dictionary<string, string> { [x.Value] = x.Key }).Append("https://ref.gs1.org/standards/epcis/2.0.0/epcis-context.jsonld"),
            ["id"] = $"fastnt:epcis:2.0:pollquery:{Guid.NewGuid()}",
            ["type"] = "EPCISQueryDocument",
            ["schemaVersion"] = "2.0",
            ["creationDate"] = DateTime.UtcNow,
            ["epcisBody"] = new
            {
                queryResults = new
                {
                    queryName = result.QueryName,
                    resultsBody = new
                    {
                        eventList = result.EventList.Select(x => JsonEventFormatter.FormatEvent(x, context))
                    }
                }
            }
        };

        return JsonSerializer.Serialize(document, Options);
    }

    // Builds a context for JSON format.
    // key=namespace, value=prefix
    // The prefixes are incremental (ext1, ext2, ext...)
    private static IDictionary<string, string> BuildContext(IEnumerable<string> namespaces, int counter = 0) => namespaces.ToDictionary(x => x, x => $"ext{counter++}");
}
