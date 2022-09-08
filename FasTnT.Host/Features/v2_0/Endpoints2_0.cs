using FasTnT.Application.Services.Users;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Queries;
using FasTnT.Host.Features.v2_0.Interfaces;
using MediatR;

namespace FasTnT.Host.Features.v2_0;

public class Endpoints2_0
{
    protected Endpoints2_0() { }

    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("v2_0/capture", HandleCaptureRequest).RequireAuthorization(policyNames: nameof(ICurrentUser.CanCapture));
        app.MapGet("v2_0/events", HandleEventQuery).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));

        return app;
    }

    private static async Task<IResult> HandleCaptureRequest(CaptureRequest request, IMediator mediator, ILogger<Endpoints2_0> logger, CancellationToken cancellationToken)
    {
        logger.LogInformation("Start capture request processing");
        await mediator.Send(request.Request, cancellationToken);

        return Results.NoContent();
    }

    private static async Task<IResult> HandleEventQuery(QueryParameters parameters, IMediator mediator, ILogger<Endpoints2_0> logger, CancellationToken cancellationToken)
    {
        var query = new PollQuery("SimpleEventQuery", parameters.Parameters);

        try
        {
            var response = await mediator.Send(query, cancellationToken) as PollResponse;

            return IRestResponse.Create(response);
        }
        catch(EpcisException ex) 
        {
            return IRestResponse.Fault(ex);
        }
    }
}

