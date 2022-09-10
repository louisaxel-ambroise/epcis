using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Queries;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FasTnT.Formatter.v2_0.Json.Formatters;

public static class JsonResponseFormatter
{
    public static string Format(IEpcisResponse response)
    {
        return response switch
        {
            PollResponse poll => FormatPoll(poll),
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

    private static string FormatPoll(PollResponse response)
    {
        var context = BuildContext(response.EventList.SelectMany(x => x.CustomFields).Select(x => x.Namespace).Distinct());
        var document = new Dictionary<string, object>
        {
            ["@context"] = context.Select(x => (object) new Dictionary<string, string> { [x.Value] = x.Key }).Append("https://ref.gs1.org/standards/epcis/2.0.0/epcis-context.jsonld"),
            ["id"] = $"fastnt:epcis:2.0:pollquery:{Guid.NewGuid()}",
            ["type"] = "EPCISQueryDocument",
            ["schemaVersion"] = "2.0",
            ["creationDate"] = DateTime.UtcNow,
            ["epcisBody"] = new
            {
                queryResults = new
                {
                    queryName = response.QueryName,
                    resultsBody = new
                    {
                        eventList = response.EventList.Select(x => JsonEventFormatter.FormatEvent(x, context))
                    }
                }
            }
        };

        return JsonSerializer.Serialize(document, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
    }

    // Builds a context for JSON format.
    // key=namespace, value=prefix
    // The prefixes are incremental (ext1, ext2, ext...)
    private static IDictionary<string, string> BuildContext(IEnumerable<string> namespaces, int counter = 0) => namespaces.ToDictionary(x => x, x => $"ext{counter++}");
}
