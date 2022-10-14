using FasTnT.Application.UseCases.Captures;
using FasTnT.Features.v2_0.Endpoints.Interfaces;
using FasTnT.Features.v2_0.Endpoints.Interfaces.Utils;

namespace FasTnT.Features.v2_0.Endpoints;

public static class CaptureEndpoints
{
    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.TryMapGet("v2_0/capture", HandleListCapturesQuery).RequireAuthorization();
        app.TryMapGet("v2_0/capture/{captureId}", HandleCaptureDetailQuery).RequireAuthorization();
        app.TryMapPost("v2_0/capture", HandleCaptureRequest).RequireAuthorization("capture");
        app.TryMapPost("v2_0/events", HandleCaptureSingleEventRequest).RequireAuthorization("capture");

        return app;
    }

    private static async Task<IResult> HandleListCapturesQuery(IListCaptureRequestsHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListCapturesAsync(cancellationToken);

        return EpcisResults.Ok(new ListCapturesResult(response));
    }

    private static async Task<IResult> HandleCaptureDetailQuery(int captureId, ICaptureRequestDetailsHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.GetCaptureDetailsAsync(captureId, cancellationToken);

        return EpcisResults.Ok(new CaptureDetailsResult(response));
    }

    private static async Task<IResult> HandleCaptureRequest(CaptureDocumentRequest request, ICaptureRequestHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.StoreAsync(request.Request, cancellationToken);

        return Results.Created($"v2_0/capture/{response.Id}", null);
    }

    private static async Task<IResult> HandleCaptureSingleEventRequest(CaptureEventRequest request, ICaptureRequestHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.StoreAsync(request.Request, cancellationToken);

        return Results.Created($"v2_0/events/{response.Events.First().EventId}", null);
    }
}
