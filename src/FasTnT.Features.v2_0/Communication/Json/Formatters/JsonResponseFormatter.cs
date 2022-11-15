using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;
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
            CollectionResult collectionResult => FormatCollection(collectionResult.Values),
            CustomQueryDefinitionResult customQuery => FormatCustomQuery(customQuery),
            ListCustomQueriesResult listQueries => FormatListQueries(listQueries),
            QueryResult queryResult => FormatQueryResult(queryResult.Response),
            ListCapturesResult captureResult => FormatListCaptures(captureResult.Captures),
            CaptureDetailsResult captureDetails => FormatCaptureDetails(captureDetails.Capture),
            ListSubscriptionsResult subscriptionsResult => FormatListSubscriptions(subscriptionsResult.Subscriptions),
            SubscriptionDetailsResult subscriptionsDetail => FormatSubscriptionDetail(subscriptionsDetail.Subscription),
            _ => FormatError(EpcisException.Default)
        };
    }

    private static string FormatCollection(IEnumerable<string> values)
    {
        var formatted = new Dictionary<string, object>
        {
            ["@context"] = "https://ref.gs1.org/standards/epcis/2.0.0/epcis-context.jsonld",
            ["type"] = "Collection",
            ["member"] = values
        };

        return JsonSerializer.Serialize(formatted, Options);
    }

    private static string FormatListSubscriptions(IEnumerable<Subscription> subscriptions)
    {
        var formatted = subscriptions.Select(FormatSubscription);

        return JsonSerializer.Serialize(formatted, Options);
    }

    private static string FormatSubscriptionDetail(Subscription subscription)
    {
        return JsonSerializer.Serialize(FormatSubscription(subscription), Options);
    }

    private static object FormatSubscription(Subscription subscription)
    {
        return new
        {
            subscriptionID = subscription.Name,
            createdAt = subscription.InitialRecordTime,
            schedule = subscription.Schedule,
            stream = subscription.Schedule == null ? true : default(bool?)
        };
    }

    public static string FormatError(EpcisException error)
    {
        return JsonSerializer.Serialize(new
        {
            type = $"epcisException:{error.ExceptionType}",
            title = error.Message,
            status = GetHttpStatusCode(error)
        });
    }

    public static int GetHttpStatusCode(EpcisException error)
    {
        return error.ExceptionType switch
        {
            ExceptionType.NoSuchNameException => 404,
            ExceptionType.NoSuchSubscriptionException => 404,
            ExceptionType.QueryTooComplexException => 413,
            ExceptionType.ImplementationException => 500,
            _ => 400
        };
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

    private static string FormatListCaptures(IEnumerable<Request> requests)
    {
        return JsonSerializer.Serialize(requests.Select(FormatCapture), Options);
    }

    private static string FormatCaptureDetails(Request request)
    {
        return JsonSerializer.Serialize(FormatCapture(request), Options);
    }

    private static object FormatCapture(Request request)
    {
        return new Dictionary<string, object>
        {
            ["captureID"] = request.Id.ToString(),
            ["createdAt"] = request.DocumentTime,
            ["finishedAt"] = request.CaptureDate,
            ["running"] = false,
            ["success"] = true,
            ["captureErrorBehaviour"] = "rollback", // TODO: handle greedy capture
            ["errors"] = Array.Empty<string>()
        };
    }

    private static string FormatQueryResult(QueryResponse result)
    {
        var context = BuildContext(result.EventList.SelectMany(x => x.Fields).Select(x => x.Namespace).Where(x => !string.IsNullOrEmpty(x)).Distinct());
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
