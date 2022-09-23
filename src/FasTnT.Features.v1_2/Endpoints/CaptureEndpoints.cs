using FasTnT.Application.Services.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using FasTnT.Features.v1_2.Endpoints.Interfaces;
using FasTnT.Application.UseCases.Captures;

namespace FasTnT.Features.v1_2.Endpoints;

public static class CaptureEndpoints
{
    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.TryMapPost("v1_2/capture", HandleCaptureRequest).RequireAuthorization(policyNames: nameof(ICurrentUser.CanCapture));

        return app;
    }

    private static async Task<IResult> HandleCaptureRequest(CaptureRequest request, ICaptureRequestHandler handler, CancellationToken cancellationToken)
    {
        await handler.StoreAsync(request.Request, cancellationToken);

        return Results.NoContent();
    }
}
