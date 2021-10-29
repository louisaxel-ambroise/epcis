using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Queries.Poll;
using FasTnT.Formatter.Xml.Formatters;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace FasTnT.Subscriptions
{
    public class HttpSubscriptionResultSender : ISubscriptionResultSender
    {
        public async Task<bool> Send<T>(string destination, T epcisResponse, CancellationToken cancellationToken)
        {
            var request = WebRequest.CreateHttp(destination);
            request.Method = "POST";
            TrySetBasicAuthorization(request);

            await WriteRequestPayload(request, epcisResponse, cancellationToken).ConfigureAwait(false);

            return await SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<bool> SendRequestAsync(HttpWebRequest request, CancellationToken cancellationToken)
        {
            var requestWasSent = default(bool);

            try
            {
                using var registration = cancellationToken.Register(() => request.Abort(), false);
                using var response = await request.GetResponseAsync() as HttpWebResponse;
                using var responseMessage = new HttpResponseMessage(response.StatusCode);

                requestWasSent = responseMessage.IsSuccessStatusCode;
            }
            catch (WebException)
            {
                requestWasSent = false;
            }

            return requestWasSent;
        }

        private static async Task WriteRequestPayload<T>(HttpWebRequest request, T response, CancellationToken cancellationToken)
        {
            using var stream = await request.GetRequestStreamAsync();
            using var writer = XmlWriter.Create(stream, new XmlWriterSettings { Async = true });

            var content = response switch
            {
                PollResponse pollResult => XmlResponseFormatter.FormatPoll(pollResult),
                EpcisException exception => XmlResponseFormatter.FormatError(exception),
                _ => throw new ArgumentException(null, nameof(response))
            };

            var requestPayload = FormatResponse(content);

            request.ContentType = "application/text+xml";
            await requestPayload.WriteToAsync(writer, cancellationToken);
        }

        private static void TrySetBasicAuthorization(HttpWebRequest request)
        {
            if (!string.IsNullOrEmpty(request.RequestUri.UserInfo))
            {
                var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(WebUtility.UrlDecode(request.RequestUri.UserInfo)));
                request.Headers.Add("Authorization", $"Basic {token}");
            }
        }

        private static XDocument FormatResponse(XElement content)
        {
            var rootName = XName.Get("EPCISQueryDocument", "urn:epcglobal:epcis-query:xsd:1");
            var attributes = new[]
            {
                new XAttribute("creationDate", DateTime.UtcNow),
                new XAttribute("schemaVersion", "1")
            };

            return new (new XElement(rootName, attributes, new XElement("EPCISBody", content)));
        }
    }
}
