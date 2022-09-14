using FasTnT.Application.Services.Capture;
using FasTnT.Application.Services.Users;
using FasTnT.Application.UseCases.CaptureRequestDetails;
using FasTnT.Features.v2_0.Endpoints.Interfaces;

namespace FasTnT.Features.v2_0.Endpoints;

public class CaptureEndpoints
{
    protected CaptureEndpoints() { }

    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("v2_0/capture/{captureId}", HandleCaptureDetailQuery).RequireAuthorization(policyNames: nameof(ICurrentUser.CanCapture));
        app.MapPost("v2_0/capture", HandleCaptureRequest).RequireAuthorization(policyNames: nameof(ICurrentUser.CanCapture));
        app.MapPost("v2_0/events", HandleCaptureSingleEventRequest).RequireAuthorization(policyNames: nameof(ICurrentUser.CanCapture));

        return app;
    }

    private static async Task<IResult> HandleCaptureDetailQuery(int captureId, ICaptureRequestDetailsHandler handler, CancellationToken cancellationToken)
    {
        var request = await handler.GetCaptureDetails(captureId, cancellationToken);

        return EpcisResults.Ok(request);
    }

    private static async Task<IResult> HandleCaptureRequest(CaptureDocumentRequest request, IStoreEpcisDocumentHandler handler, CancellationToken cancellationToken)
    {
        await handler.StoreAsync(request.Request, cancellationToken);

        return Results.Created($"v2_0/capture/{request.Request.Id}", null);
    }

    private static async Task<IResult> HandleCaptureSingleEventRequest(CaptureEventRequest request, IStoreEpcisDocumentHandler handler, CancellationToken cancellationToken)
    {
        await handler.StoreAsync(request.Request, cancellationToken);

        return Results.Created($"v2_0/events/{request.Request.Events.First().EventId}", null);
    }
}

