using FasTnT.Application.Handlers;
using FasTnT.Host.Endpoints.Interfaces;
using FasTnT.Host.Endpoints.Responses.Rest;
using FasTnT.Host.Extensions;

namespace FasTnT.Host.Endpoints;

public static class CaptureEndpoints
{
    public static IEndpointRouteBuilder AddCaptureEndpoints(this IEndpointRouteBuilder app)
    {
        app.Get("capture", ListCapturesQuery).RequireAuthorization();
        app.Get("capture/{captureId}", CaptureDetailQuery).RequireAuthorization();
        app.Post("capture", CaptureRequest).RequireAuthorization("capture");
        app.Post("events", CaptureSingleEventRequest).RequireAuthorization("capture");

        return app;
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

        return Results.Created($"capture/{response.CaptureId}", null);
    }

    private static async Task<IResult> CaptureSingleEventRequest(CaptureEventRequest request, CaptureHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.StoreAsync(request.Request, cancellationToken);

        return Results.Created($"events/{response.Events.First().EventId}", null);
    }
}
