using FasTnT.Application.UseCases.Captures;
using FasTnT.Host.Features.v2_0.Endpoints.Interfaces;
using FasTnT.Host.Features.v2_0.Endpoints.Interfaces.Utils;

namespace FasTnT.Host.Features.v2_0.Endpoints;

public static class CaptureEndpoints
{
    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.Get("v2_0/capture", HandleListCapturesQuery).RequireAuthorization();
        app.Get("v2_0/capture/{captureId}", HandleCaptureDetailQuery).RequireAuthorization();
        app.Post("v2_0/capture", HandleCaptureRequest).RequireAuthorization("capture");
        app.Post("v2_0/events", HandleCaptureSingleEventRequest).RequireAuthorization("capture");

        return app;
    }

    private static async Task<IResult> HandleListCapturesQuery(PaginationContext context, IListCaptureRequestsHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListCapturesAsync(context.Pagination, cancellationToken);

        return EpcisResults.Ok(new ListCapturesResult(response));
    }

    private static async Task<IResult> HandleCaptureDetailQuery(string captureId, ICaptureRequestDetailsHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.GetCaptureDetailsAsync(captureId, cancellationToken);

        return EpcisResults.Ok(new CaptureDetailsResult(response));
    }

    private static async Task<IResult> HandleCaptureRequest(CaptureDocumentRequest request, ICaptureRequestHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.StoreAsync(request.Request, cancellationToken);

        return Results.Created($"v2_0/capture/ {response.CaptureId}", null);
    }

    private static async Task<IResult> HandleCaptureSingleEventRequest(CaptureEventRequest request, ICaptureRequestHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.StoreAsync(request.Request, cancellationToken);

        return Results.Created($"v2_0/events/{response.Events.First().EventId}", null);
    }
}
