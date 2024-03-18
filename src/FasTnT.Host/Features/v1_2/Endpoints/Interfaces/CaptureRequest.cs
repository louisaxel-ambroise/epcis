using FasTnT.Domain.Model;
using FasTnT.Host.Communication.Xml.Parsers;

namespace FasTnT.Host.Features.v1_2.Endpoints.Interfaces;

public record CaptureRequest(Request Request)
{
    public static async ValueTask<CaptureRequest> BindAsync(HttpContext context)
    {
        return await XmlCaptureRequestParser.ParseAsync(context.Request.Body, context.RequestAborted);
    }

    public static implicit operator CaptureRequest(Request request) => new(request);
}
