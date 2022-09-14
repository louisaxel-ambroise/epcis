using FasTnT.Application.Services.Users;

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

    private static Task HandleListEventTypes(HttpContext context)
    {
        throw new NotImplementedException();
    }

    private static Task HandleListEpcs(HttpContext context)
    {
        throw new NotImplementedException();
    }

    private static Task HandleListBizSteps(HttpContext context)
    {
        throw new NotImplementedException();
    }

    private static Task HandleListBizLocations(HttpContext context)
    {
        throw new NotImplementedException();
    }

    private static Task HandleListReadPoints(HttpContext context)
    {
        throw new NotImplementedException();
    }

    private static Task HandleListDispositions(HttpContext context)
    {
        throw new NotImplementedException();
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

