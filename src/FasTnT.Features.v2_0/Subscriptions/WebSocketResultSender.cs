using FasTnT.Application.Services.Subscriptions;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Features.v2_0.Communication.Json.Formatters;
using FasTnT.Features.v2_0.Endpoints.Interfaces;
using System.Net.WebSockets;
using System.Text;

namespace FasTnT.Features.v2_0.Subscriptions;

public class WebSocketResultSender : IResultSender
{
    private readonly WebSocket _webSocket;

    public string Name => nameof(WebSocketResultSender);

    public WebSocketResultSender(WebSocket webSocket)
    {
        _webSocket = webSocket;
    }

    public async Task<bool> SendResultAsync(Subscription context, QueryResponse response, CancellationToken cancellationToken)
    {
        var formattedResponse = JsonResponseFormatter.Format(new QueryResult(response));
        var responseByteArray = Encoding.UTF8.GetBytes(formattedResponse);

        try
        {
            await _webSocket.SendAsync(responseByteArray, WebSocketMessageType.Text, true, cancellationToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SendErrorAsync(Subscription context, EpcisException error, CancellationToken cancellationToken)
    {
        var formattedResponse = JsonResponseFormatter.FormatError(error);
        var responseByteArray = Encoding.UTF8.GetBytes(formattedResponse);

        try
        {
            await _webSocket.SendAsync(responseByteArray, WebSocketMessageType.Text, true, cancellationToken);

            return true;
        }
        catch
        {
            return false;
        }
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
