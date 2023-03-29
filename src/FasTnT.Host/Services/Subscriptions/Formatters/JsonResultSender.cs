using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Host.Features.v2_0.Communication.Json.Formatters;
using FasTnT.Host.Features.v2_0.Communication.Json.Utils;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FasTnT.Host.Services.Subscriptions.Formatters;

public class JsonResultSender : IResultSender
{
    private static readonly JsonSerializerOptions Options = new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
    public static readonly IResultSender Instance = new JsonResultSender();

    public string Name => nameof(JsonResultSender);

    private JsonResultSender() { }

    public Task<bool> SendResultAsync(Subscription context, QueryResponse response, CancellationToken cancellationToken)
    {
        using var client = GetHttpClient(context.Destination, context.SignatureToken);
        var formattedResponse = FormatEpcisResult(response, context);

        return SendRequestAsync(client, formattedResponse, cancellationToken);
    }

    public Task<bool> SendErrorAsync(Subscription context, EpcisException error, CancellationToken cancellationToken)
    {
        using var client = GetHttpClient(context.Destination, context.SignatureToken);
        var formattedResponse = JsonSerializer.Serialize(new
        {
            type = $"epcisException:{error.ExceptionType}",
            title = error.Message,
            status = 400
        });

        return SendRequestAsync(client, formattedResponse, cancellationToken);
    }

    private static string FormatEpcisResult(QueryResponse response, Subscription subscription)
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
                    queryName = subscription.QueryName,
                    subscriptionID = subscription.Name,
                    resultsBody = new
                    {
                        eventList = response.EventList.Select(x => JsonEventFormatter.FormatEvent(x, context))
                    }
                }
            }
        };

        return JsonSerializer.Serialize(document, Options);
    }

    private static async Task<bool> SendRequestAsync(HttpClient request, string content, CancellationToken cancellationToken)
    {
        using var httpContent = new StringContent(content, Encoding.UTF8, "application/json");

        try
        {
            var httpResponse = await request.PostAsync(string.Empty, httpContent, cancellationToken);
            httpResponse.EnsureSuccessStatusCode();

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static HttpClient GetHttpClient(string destination, string signatureToken)
    {
        var client = new HttpClient { BaseAddress = new Uri(destination) };

        if (!string.IsNullOrEmpty(signatureToken))
        {
            client.DefaultRequestHeaders.Add("GS1-Signature", signatureToken);
        }

        return client;
    }
}
