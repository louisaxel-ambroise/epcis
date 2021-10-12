using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Queries.Poll;
using FasTnT.Formatter.Xml.Formatters;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

            await WriteRequestPayload(request, epcisResponse, cancellationToken);

            return await SendRequestAsync(request, cancellationToken);
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
            catch (WebException ex)
            {
                requestWasSent = false;
            }

            return requestWasSent;
        }

        private static async Task WriteRequestPayload<T>(HttpWebRequest request, T response, CancellationToken cancellationToken)
        {
            using var stream = await request.GetRequestStreamAsync();

            var content = response switch
            {
                PollResponse pollResult => XmlResponseFormatter.FormatPoll(pollResult),
                EpcisException exception => XmlResponseFormatter.FormatError(exception),
                _ => throw new ArgumentException(null, nameof(response))
            };

            var payload = new XDocument(new XElement(XName.Get("EPCISQueryDocument", "urn:epcglobal:epcis-query:xsd:1"), 
                new XAttribute("creationDate", DateTime.UtcNow), new XAttribute("schemaVersion", "1"),
                new XElement("EPCISBody", content)));

            request.ContentType = "application/text+xml";
            var requestPayload = payload.ToString();
            request.GetRequestStream().Write(Encoding.UTF8.GetBytes(requestPayload), 0, requestPayload.Length);
        }

        private static void TrySetBasicAuthorization(HttpWebRequest request)
        {
            if (!string.IsNullOrEmpty(request.RequestUri.UserInfo))
            {
                var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(WebUtility.UrlDecode(request.RequestUri.UserInfo)));
                request.Headers.Add("Authorization", $"Basic {token}");
            }
        }
    }
}
