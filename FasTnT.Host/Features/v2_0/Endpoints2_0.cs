using FasTnT.Application.Services.Users;
using FasTnT.Domain.Commands.Capture;
using FasTnT.Formatter.Json;
using MediatR;

namespace FasTnT.Host.Features.v2_0;

public class Endpoints2_0
{
    protected Endpoints2_0() { }

    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("v2_0/capture", HandleCaptureRequest).RequireAuthorization(policyNames: nameof(ICurrentUser.CanCapture));
        app.MapGet("v2_0/capture", HandleCaptureQuery).RequireAuthorization(policyNames: nameof(ICurrentUser.CanCapture));

        return app;
    }

    private static async Task<IResult> HandleCaptureRequest(CaptureRequest request, IMediator mediator, ILogger<Endpoints2_0> logger, CancellationToken cancellationToken)
    {
        logger.LogInformation("Start capture request processing");
        await mediator.Send(request.Request, cancellationToken);

        return Results.NoContent();
    }

    private static async Task<IResult> HandleCaptureQuery(IMediator mediator, ILogger<Endpoints2_0> logger, CancellationToken cancellationToken)
    {
        return Results.Ok();
    }
}

public record CaptureRequest(IRequest<CaptureEpcisRequestResponse> Request)
{
    public static async ValueTask<CaptureRequest> BindAsync(HttpContext context)
    {
        var extensions = ParseExtensionHeader(context);
        var request = await CaptureRequestParser.ParseAsync(context.Request.Body, extensions, context.RequestAborted);

        return new(request);
    }

    private static IDictionary<string, string> ParseExtensionHeader(HttpContext context)
    {
        if(context.Request.Headers.TryGetValue("GS1-Extensions", out var extensions))
        {
            return extensions
                .Select(x => x.Split('=', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                .ToDictionary(x => x[0], x => x[1]);
        }

        return new Dictionary<string, string>();
    }
}

