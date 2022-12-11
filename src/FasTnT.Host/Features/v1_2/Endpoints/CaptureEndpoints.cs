using FasTnT.Application.UseCases.Captures;
using FasTnT.Host.Features.v1_2.Endpoints.Interfaces;

namespace FasTnT.Host.Features.v1_2.Endpoints;

public static class CaptureEndpoints
{
    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.TryMapPost("v1_2/capture", HandleCaptureRequest).RequireAuthorization("capture");

        return app;
    }

    private static async Task<IResult> HandleCaptureRequest(CaptureRequest request, ICaptureRequestHandler handler, CancellationToken cancellationToken)
    {
        await handler.StoreAsync(request.Request, cancellationToken);

        return Results.NoContent();
    }
}
