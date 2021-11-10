using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Queries;
using FasTnT.Formatter.Xml.Formatters;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace FasTnT.Subscriptions;

public class HttpSubscriptionResultSender : ISubscriptionResultSender
{
    public async Task<bool> Send<T>(SubscriptionExecutionContext context, T response, CancellationToken cancellationToken)
    {
        using var client = GetHttpClient(context.Subscription.Destination);
        using var stream = await GetResponseStream(response, context.DateTime, cancellationToken);

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

    private static async Task<Stream> GetResponseStream<T>(T response, DateTime executionDate, CancellationToken cancellationToken)
    {
        var stream = new MemoryStream();

        using var writer = XmlWriter.Create(stream, new XmlWriterSettings { Async = true, CloseOutput = false });

        var content = response switch
        {
            PollResponse pollResult => XmlResponseFormatter.FormatPoll(pollResult),
            EpcisException exception => XmlResponseFormatter.FormatError(exception),
            _ => throw new ArgumentException("Unexpected subscription response type", nameof(response))
        };

        var requestPayload = FormatResponse(content, executionDate);
        await requestPayload.WriteToAsync(writer, cancellationToken);
        await writer.FlushAsync();

        stream.Seek(0, SeekOrigin.Begin);

        return stream;
    }

    private static HttpClient GetHttpClient(string destination)
    {
        var client = new HttpClient() { BaseAddress = new Uri(destination) };

        if (!string.IsNullOrEmpty(client.BaseAddress.UserInfo))
        {
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(WebUtility.UrlDecode(client.BaseAddress.UserInfo)));
            client.DefaultRequestHeaders.Add("Authorization", $"Basic {token}");
        }

        return client;
    }

    private static XDocument FormatResponse(XElement content, DateTime executionDate)
    {
        var rootName = XName.Get("EPCISQueryDocument", "urn:epcglobal:epcis-query:xsd:1");
        var attributes = new[]
        {
            new XAttribute("creationDate", executionDate),
            new XAttribute("schemaVersion", "1")
        };

        return new (new XElement(rootName, attributes, new XElement("EPCISBody", content)));
    }
}
