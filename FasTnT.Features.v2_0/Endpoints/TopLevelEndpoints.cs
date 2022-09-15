using FasTnT.Application.Services.Users;
using FasTnT.Application.UseCases.ListTopLevelResources;

namespace FasTnT.Features.v2_0.Endpoints;

public class TopLevelEndpoints
{
    protected TopLevelEndpoints() { }

    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("v2_0/eventTypes", HandleListEventTypes).RequireAuthorization(nameof(ICurrentUser.CanQuery));
        app.MapGet("v2_0/epcs", HandleListEpcs).RequireAuthorization(nameof(ICurrentUser.CanQuery));
        app.MapGet("v2_0/bizSteps", HandleListBizSteps).RequireAuthorization(nameof(ICurrentUser.CanQuery));
        app.MapGet("v2_0/bizLocations", HandleListBizLocations).RequireAuthorization(nameof(ICurrentUser.CanQuery));
        app.MapGet("v2_0/readPoints", HandleListReadPoints).RequireAuthorization(nameof(ICurrentUser.CanQuery));
        app.MapGet("v2_0/dispositions", HandleListDispositions).RequireAuthorization(nameof(ICurrentUser.CanQuery));
        app.MapGet("v2_0/eventTypes/{eventType}", HandleSubResourceRequest).RequireAuthorization(nameof(ICurrentUser.CanQuery));
        app.MapGet("v2_0/epcs/{epc}", HandleSubResourceRequest).RequireAuthorization(nameof(ICurrentUser.CanQuery));
        app.MapGet("v2_0/bizSteps/{bizStep}", HandleSubResourceRequest).RequireAuthorization(nameof(ICurrentUser.CanQuery));
        app.MapGet("v2_0/bizLocations/{bizLocation}", HandleSubResourceRequest).RequireAuthorization(nameof(ICurrentUser.CanQuery));
        app.MapGet("v2_0/readPoints/{readPoint}", HandleSubResourceRequest).RequireAuthorization(nameof(ICurrentUser.CanQuery));
        app.MapGet("v2_0/dispositions/{disposition}", HandleSubResourceRequest).RequireAuthorization(nameof(ICurrentUser.CanQuery));

        return app;
    }

    private static async Task<IResult> HandleListEventTypes(IListEventTypesHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListEventTypes(cancellationToken);

        return Results.Ok(response);
    }

    private static async Task<IResult> HandleListEpcs(IListEpcsHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListEpcs(cancellationToken);

        return Results.Ok(response);
    }

    private static async Task<IResult> HandleListBizSteps(IListBizStepsHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListBizSteps(cancellationToken);

        return Results.Ok(response);
    }

    private static async Task<IResult> HandleListBizLocations(IListBizLocationsHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListBizLocations(cancellationToken);

        return Results.Ok(response);
    }

    private static async Task<IResult> HandleListReadPoints(IListReadPointsHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListReadPoints(cancellationToken);

        return Results.Ok(response);
    }

    private static async Task<IResult> HandleListDispositions(IListDispositionsHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListDispositions(cancellationToken);

        return Results.Ok(response);
    }

    private static IResult HandleSubResourceRequest()
    {
        return Results.Ok(new Dictionary<string, object>
        {
            ["@context"] = "https://ref.gs1.org/standards/epcis/2.0.0/epcis-context.jsonld",
            ["type"] = "collection",
            ["member"] = new[] { "events" }
        });
    }
}

