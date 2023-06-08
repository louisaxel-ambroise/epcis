using FasTnT.Application.Handlers;
using FasTnT.Host.Features.v2_0.Endpoints.Interfaces;
using FasTnT.Host.Features.v2_0.Endpoints.Interfaces.Utils;

namespace FasTnT.Host.Features.v2_0.Endpoints;

public static class CaptureEndpoints
{
    public static void AddRoutes(IEndpointRouteBuilder app)
    {
        app.Get("v2_0/capture", ListCapturesQuery).RequireAuthorization();
        app.Get("v2_0/capture/{captureId}", CaptureDetailQuery).RequireAuthorization();
        app.Post("v2_0/capture", CaptureRequest).RequireAuthorization("capture");
        app.Post("v2_0/events", CaptureSingleEventRequest).RequireAuthorization("capture");
    }

    private static async Task<IResult> ListCapturesQuery(PaginationContext context, CaptureHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListCapturesAsync(context.Pagination, cancellationToken);

        return EpcisResults.Ok(new ListCapturesResult(response));
    }

    private static async Task<IResult> CaptureDetailQuery(string captureId, CaptureHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.GetCaptureDetailsAsync(captureId, cancellationToken);

        return EpcisResults.Ok(new CaptureDetailsResult(response));
    }

    private static async Task<IResult> CaptureRequest(CaptureDocumentRequest request, CaptureHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.StoreAsync(request.Request, cancellationToken);

        return Results.Created($"v2_0/capture/{response.CaptureId}", null);
    }

    private static async Task<IResult> CaptureSingleEventRequest(CaptureEventRequest request, CaptureHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.StoreAsync(request.Request, cancellationToken);

        return Results.Created($"v2_0/events/{response.Events.First().EventId}", null);
    }
}
