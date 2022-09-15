using FasTnT.Application.Services.Users;
using FasTnT.Application.UseCases.CaptureRequestDetails;
using FasTnT.Application.UseCases.ListCaptureRequests;
using FasTnT.Application.UseCases.StoreEpcisDocument;
using FasTnT.Features.v2_0.Endpoints.Interfaces;
using FasTnT.Features.v2_0.Endpoints.Interfaces.Utils;

namespace FasTnT.Features.v2_0.Endpoints;

public class CaptureEndpoints
{
    protected CaptureEndpoints() { }

    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("v2_0/capture", HandleListCapturesQuery).RequireAuthorization(policyNames: nameof(ICurrentUser.CanCapture));
        app.MapGet("v2_0/capture/{captureId}", HandleCaptureDetailQuery).RequireAuthorization(policyNames: nameof(ICurrentUser.CanCapture));
        app.MapPost("v2_0/capture", HandleCaptureRequest).RequireAuthorization(policyNames: nameof(ICurrentUser.CanCapture));
        app.MapPost("v2_0/events", HandleCaptureSingleEventRequest).RequireAuthorization(policyNames: nameof(ICurrentUser.CanCapture));

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

    private static async Task<IResult> HandleCaptureRequest(CaptureDocumentRequest request, IStoreEpcisDocumentHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.StoreAsync(request.Request, cancellationToken);

        return Results.Created($"v2_0/capture/{response.Id}", null);
    }

    private static async Task<IResult> HandleCaptureSingleEventRequest(CaptureEventRequest request, IStoreEpcisDocumentHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.StoreAsync(request.Request, cancellationToken);

        return Results.Created($"v2_0/events/{response.Events.First().EventId}", null);
    }
}

