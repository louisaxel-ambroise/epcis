using FasTnT.Domain.Commands.Capture;
using FasTnT.Formatter.Xml.Parsers;
using MediatR;

namespace FasTnT.Host.Features.v1_2;

public record CaptureRequest(IRequest<CaptureEpcisRequestResponse> Request)
{
    public static async ValueTask<CaptureRequest> BindAsync(HttpContext context)
    {
        var request = await CaptureRequestParser.ParseAsync(context.Request.Body, context.RequestAborted);

        return new(request);
    }
}
