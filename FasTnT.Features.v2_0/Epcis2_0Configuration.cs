using FasTnT.Features.v2_0.Endpoints;

namespace FasTnT.Features.v2_0;

public static class Epcis2_0Configuration
{
    public static IEndpointRouteBuilder MapEpcis20Endpoints(this IEndpointRouteBuilder endpoints)
    {
        CaptureEndpoints.AddRoutes(endpoints);
        EventsEndpoints.AddRoutes(endpoints);
        QueriesEndpoints.AddRoutes(endpoints);
        TopLevelEndpoints.AddRoutes(endpoints);
        SubscriptionEndpoints.AddRoutes(endpoints);
        DiscoveryEndpoints.AddRoutes(endpoints);

        return endpoints;
    }
}
