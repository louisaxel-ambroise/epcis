using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Queries;
using FasTnT.Host.Features.v2_0.Communication.Json.Formatters;
using FasTnT.Host.Features.v2_0.Communication.Json.Utils;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FasTnT.Host.Subscriptions.Formatters;

public class JsonSubscriptionFormatter : ISubscriptionFormatter
{
    private static readonly JsonSerializerOptions Options = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
    public static readonly ISubscriptionFormatter Instance = new JsonSubscriptionFormatter();

    public string ContentType => "application/json";

    private JsonSubscriptionFormatter() { }

    public string FormatError(string name, string query, EpcisException error)
    {
        return JsonSerializer.Serialize(new
        {
            type = $"epcisException:{error.ExceptionType}",
            title = error.Message,
            status = 400
        });
    }

    public string FormatResult(string name, QueryResponse response)
    {
        var context = response.EventList.SelectMany(x => x.Fields).Select(x => x.Namespace).BuildContext();
        var document = new Dictionary<string, object>
        {
            ["@context"] = context.Select(x => (object)new Dictionary<string, string> { [x.Value] = x.Key }).Append("https://ref.gs1.org/standards/epcis/2.0.0/epcis-context.jsonld"),
            ["id"] = $"fastnt:epcis:2.0:subscription:{Guid.NewGuid()}",
            ["type"] = "EPCISQueryDocument",
            ["schemaVersion"] = "2.0",
            ["creationDate"] = DateTime.UtcNow,
            ["epcisBody"] = new
            {
                queryResults = new
                {
                    queryName = response.QueryName,
                    subscriptionID = name,
                    resultsBody = new
                    {
                        eventList = response.EventList.Select(x => JsonEventFormatter.FormatEvent(x, context))
                    }
                }
            }
        };

        return JsonSerializer.Serialize(document, Options);
    }
}
