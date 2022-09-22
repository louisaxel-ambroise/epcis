using FasTnT.Application.Services.Subscriptions;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Features.v2_0.Communication.Json.Formatters;
using FasTnT.Features.v2_0.Endpoints.Interfaces;
using System.Text;

namespace FasTnT.Features.v2_0.Subscriptions;

public class JsonResultSender : IResultSender
{
    public static readonly IResultSender Instance = new JsonResultSender();

    public string Name => nameof(JsonResultSender);

    private JsonResultSender() { }

    public Task<bool> SendResultAsync(Subscription context, QueryResponse response, CancellationToken cancellationToken)
    {
        using var client = GetHttpClient(context.Destination, context.SignatureToken);
        var formattedResponse = JsonResponseFormatter.Format(new QueryResult(response));

        return SendRequestAsync(client, formattedResponse, cancellationToken);
    }

    public Task<bool> SendErrorAsync(Subscription context, EpcisException error, CancellationToken cancellationToken)
    {
        using var client = GetHttpClient(context.Destination, context.SignatureToken);
        var formattedResponse = JsonResponseFormatter.FormatError(error);

        return SendRequestAsync(client, formattedResponse, cancellationToken);
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
