using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Features.v2_0.Communication.Json.Formatters;
using FasTnT.Features.v2_0.Endpoints.Interfaces;
using System.Text;

namespace FasTnT.Features.v2_0.Subscriptions;

public class HttpSubscriptionResultSender : ISubscriptionResultSender
{
    public async Task<bool> Send<T>(SubscriptionExecutionContext context, T response, CancellationToken cancellationToken)
    {
        using var client = GetHttpClient(context.Subscription.Destination, context.Subscription.SignatureToken);

        return await SendRequestAsync(client, Format(response), cancellationToken).ConfigureAwait(false);
    }

    private static async Task<bool> SendRequestAsync(HttpClient request, string content, CancellationToken cancellationToken)
    {
        var httpContent = new StringContent(content, Encoding.UTF8, "application/json");

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

    private static string Format<T>(T response)
    {
        return response switch
        {
            QueryResult queryResult => JsonResponseFormatter.Format(queryResult),
            EpcisException exception => JsonResponseFormatter.FormatError(exception),
            _ => throw new ArgumentException("Unexpected subscription response type", nameof(response))
        };
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
