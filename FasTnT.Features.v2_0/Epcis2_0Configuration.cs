using FasTnT.Features.v2_0.Endpoints;

namespace FasTnT.Features.v2_0;

public static class Epcis2_0Configuration
{
    public static IEndpointRouteBuilder MapEpcis20Endpoints(this IEndpointRouteBuilder endpoints)
    {
        CaptureEndpoints.AddRoutes(endpoints);
        QueryEndpoints.AddRoutes(endpoints);

        return endpoints;
    }
}
