using FasTnT.Application.Domain.Model;
using FasTnT.Application.Domain.Model.Events;
using FasTnT.Host.Features.v2_0.Communication;
using FasTnT.Host.Features.v2_0.Communication.Parsers;

namespace FasTnT.Host.Features.v2_0.Endpoints.Interfaces;

public record CaptureEventRequest(Request Request)
{
    public static async ValueTask<CaptureEventRequest> BindAsync(HttpContext context)
    {
        var headerContext = context.Request.Headers.TryGetValue("GS1-Extensions", out var extensions) ? extensions.ToArray() : Array.Empty<string>();
        var epcisContext = Namespaces.ParseHeader(headerContext);

        var document = await JsonDocumentParser.Instance.ParseAsync(context.Request.Body, context.RequestAborted);
        var request = new Request
        {
            CaptureTime = DateTime.UtcNow,
            DocumentTime = DateTime.UtcNow,
            SchemaVersion = "2.0",
            Events = new List<Event> { JsonEventParser.Create(document.RootElement, epcisContext).Parse() }
        };

        return request;
    }

    public static implicit operator CaptureEventRequest(Request request) => new(request);
}
