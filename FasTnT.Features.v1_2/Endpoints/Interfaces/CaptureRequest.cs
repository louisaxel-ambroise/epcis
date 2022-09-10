using FasTnT.Domain.Commands.Capture;
using FasTnT.Features.v1_2.Communication.Parsers;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FasTnT.Features.v1_2.Endpoints.Interfaces;

public record CaptureRequest(IRequest<CaptureEpcisRequestResponse> Request)
{
    public static async ValueTask<CaptureRequest> BindAsync(HttpContext context)
    {
        var request = await CaptureRequestParser.ParseAsync(context.Request.Body, context.RequestAborted);

        return new(request);
    }
}
