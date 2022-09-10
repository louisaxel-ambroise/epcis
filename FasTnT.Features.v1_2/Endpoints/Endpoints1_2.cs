using FasTnT.Application.Services.Users;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Notifications;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Microsoft.AspNetCore.Builder;

namespace FasTnT.Host.Features.v1_2;

public class Endpoints1_2
{
    internal const string WsdlPath = "FasTnT.Host.Features.v1_2.Artifacts.epcis1_2.wsdl";

    protected Endpoints1_2() { }
    
    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("v1_2/capture", HandleCaptureRequest).RequireAuthorization(policyNames: nameof(ICurrentUser.CanCapture));
        app.MapPost("v1_2/query.svc", HandleSoapQuery).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));
        app.MapGet("v1_2/Trigger", HandleTriggerSubscription).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));
        app.MapGet("v1_2/query.svc", HandleGetWsdl).AllowAnonymous();

        return app;
    }

    private static async Task<IResult> HandleCaptureRequest(CaptureRequest request, IMediator mediator, ILogger<Endpoints1_2> logger, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Start capture request processing");
            await mediator.Send(request.Request, cancellationToken);

            return Results.NoContent();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to process capture request");

            return (ex is FormatException or EpcisException)
                ? Results.BadRequest()
                : Results.StatusCode(500);
        }
    }

    private static async Task<ISoapResponse> HandleSoapQuery(SoapEnvelope envelope, IMediator mediator, ILogger<Endpoints1_2> logger, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Start processing {queryName} query request", envelope.Query.GetType().Name);

            return ISoapResponse.Create(await mediator.Send(envelope.Query, cancellationToken));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to process query");

            return ISoapResponse.Fault(ex is EpcisException epcisEx ? epcisEx : EpcisException.Default);
        }
    }

    private static async Task<IResult> HandleTriggerSubscription(string triggers, IMediator mediator, ILogger<Endpoints1_2> logger, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(triggers))
        {
            logger.LogInformation("Trigger subscription executions: {triggers}", triggers);

            await mediator.Publish(new TriggerSubscriptionNotification(triggers.Split(',')), cancellationToken);

            return Results.NoContent();
        }
        else
        {
            return Results.BadRequest();
        }
    }

    private static async Task HandleGetWsdl(HttpResponse response, ILogger<Endpoints1_2> logger, CancellationToken cancellationToken)
    {
        logger.LogInformation("Return Query 1.2 WSDL from GET request");
        response.ContentType = "text/xml";

        await using var wsdl = Assembly.GetExecutingAssembly().GetManifestResourceStream(WsdlPath);
        await wsdl.CopyToAsync(response.Body, cancellationToken).ConfigureAwait(false);
    }
}
