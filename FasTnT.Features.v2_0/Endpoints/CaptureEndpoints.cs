using FasTnT.Application.Services.Users;
using MediatR;

namespace FasTnT.Host.Features.v2_0;

public class CaptureEndpoints
{
    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("v2_0/capture", HandleCaptureRequest).RequireAuthorization(policyNames: nameof(ICurrentUser.CanCapture));
        app.MapPost("v2_0/events", HandleCaptureSingleEventRequest).RequireAuthorization(policyNames: nameof(ICurrentUser.CanCapture));

        return app;
    }

    private static async Task<IResult> HandleCaptureRequest(CaptureDocumentRequest request, IMediator mediator, ILogger<CaptureEndpoints> logger, CancellationToken cancellationToken)
    {
        logger.LogInformation("Start capture request processing");
        await mediator.Send(request.Request, cancellationToken);

        return Results.NoContent();
    }

    private static async Task<IResult> HandleCaptureSingleEventRequest(CaptureEventRequest request, IMediator mediator, ILogger<CaptureEndpoints> logger, CancellationToken cancellationToken)
    {
        logger.LogInformation("Start capture request processing");
        await mediator.Send(request.Request, cancellationToken);

        return Results.NoContent();
    }
}

