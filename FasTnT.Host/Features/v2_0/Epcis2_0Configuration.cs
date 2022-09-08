namespace FasTnT.Host.Features.v2_0;

public static class Epcis2_0Configuration
{
    public static IEndpointRouteBuilder MapEpcis20Endpoints(this IEndpointRouteBuilder endpoints)
    {
        Endpoints2_0.AddRoutes(endpoints);

        return endpoints;
    }
}
