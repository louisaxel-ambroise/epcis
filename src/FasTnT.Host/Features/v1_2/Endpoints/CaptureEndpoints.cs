using FasTnT.Application.Handlers;
using FasTnT.Host.Features.v1_2.Endpoints.Interfaces;

namespace FasTnT.Host.Features.v1_2.Endpoints;

public static class CaptureEndpoints
{
    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.TryMapPost("v1_2/capture", CaptureRequest).RequireAuthorization("capture");

        return app;
    }

    private static async Task<IResult> CaptureRequest(CaptureRequest request, CaptureHandler handler, CancellationToken cancellationToken)
    {
        await handler.StoreAsync(request.Request, cancellationToken);

        return Results.NoContent();
    }
}
