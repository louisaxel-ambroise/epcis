using FasTnT.Application.Services.Users;
using FasTnT.Features.v2_0.Endpoints.Interfaces;
using FasTnT.Features.v2_0.Endpoints.Interfaces.Utils;

namespace FasTnT.Features.v2_0.Endpoints;

public static class DiscoveryEndpoints
{
    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.TryMapGet("v2_0/", HandleTopLevelResources).RequireAuthorization(nameof(ICurrentUser.CanQuery));

        return app;
    }

    private static Task<IResult> HandleTopLevelResources()
    {
        var resources = new[]
        {
            "queries",
            "capture",
            "events",
            "eventTypes",
            "epcs",
            "readPoints",
            "bizLocations",
            "dispositions",
            "bizSteps"
        };

        return Task.FromResult(EpcisResults.Ok(new CollectionResult(resources)));
    }
}
