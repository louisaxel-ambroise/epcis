using FasTnT.Application.Services.Subscriptions;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;
using FasTnT.Features.v1_2.Communication.Formatters;
using FasTnT.Features.v1_2.Endpoints.Interfaces;
using System.Net;
using System.Text;
using System.Xml;

namespace FasTnT.Features.v1_2.Communication;

public class XmlResultSender : IResultSender
{
    public static readonly IResultSender Instance = new XmlResultSender();

    public string Name => nameof(XmlResultSender);

    private XmlResultSender() { }

    public async Task<bool> SendResultAsync(Subscription context, QueryResponse response, CancellationToken cancellationToken)
    {
        var formattedResponse = XmlResponseFormatter.FormatPoll(new PollResult(response));

        using var client = GetHttpClient(context.Destination);
        using var stream = await GetResponseStream(formattedResponse, cancellationToken);

        return await SendRequestAsync(client, stream, cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> SendErrorAsync(Subscription context, EpcisException error, CancellationToken cancellationToken)
    {
        var formattedResponse = XmlResponseFormatter.FormatError(error);
        using var client = GetHttpClient(context.Destination);
        using var stream = await GetResponseStream(formattedResponse, cancellationToken);

        return await SendRequestAsync(client, stream, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<bool> SendRequestAsync(HttpClient request, Stream stream, CancellationToken cancellationToken)
    {
        var httpContent = new StreamContent(stream);
        httpContent.Headers.Add("Content-Type", "application/text+xml");

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

    private static async Task<Stream> GetResponseStream(XElement content, CancellationToken cancellationToken)
    {
        var stream = new MemoryStream();

        using var writer = XmlWriter.Create(stream, new XmlWriterSettings { Async = true, CloseOutput = false });

        var requestPayload = FormatResponse(content);

        await requestPayload.WriteToAsync(writer, cancellationToken);
        await writer.FlushAsync();

        stream.Seek(0, SeekOrigin.Begin);

        return stream;
    }

    private static HttpClient GetHttpClient(string destination)
    {
        var client = new HttpClient { BaseAddress = new Uri(destination) };

        if (!string.IsNullOrEmpty(client.BaseAddress.UserInfo))
        {
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(WebUtility.UrlDecode(client.BaseAddress.UserInfo)));
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {token}");
        }

        return client;
    }

    private static XDocument FormatResponse(XElement content)
    {
        var rootName = XName.Get("EPCISQueryDocument", "urn:epcglobal:epcis-query:xsd:1");
        var attributes = new[]
        {
            new XAttribute("creationDate", DateTime.UtcNow),
            new XAttribute("schemaVersion", "1")
        };

        return new(new XElement(rootName, attributes, new XElement("EPCISBody", content)));
    }
}
