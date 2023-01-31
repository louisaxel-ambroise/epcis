using FasTnT.Host.Features.v2_0.Endpoints;

namespace FasTnT.Host.Features.v2_0;

public static class Epcis2_0Configuration
{
    public static IEndpointRouteBuilder UseEpcis20Endpoints(this IEndpointRouteBuilder endpoints)
    {
        CaptureEndpoints.AddRoutes(endpoints);
        EventsEndpoints.AddRoutes(endpoints);
        QueriesEndpoints.AddRoutes(endpoints);
        TopLevelEndpoints.AddRoutes(endpoints);
        SubscriptionEndpoints.AddRoutes(endpoints);
        DiscoveryEndpoints.AddRoutes(endpoints);

        return endpoints;
    }

    internal static RouteHandlerBuilder Get(this IEndpointRouteBuilder endpoints, string pattern, Delegate handler)
    {
        return endpoints.MapMethod(HttpMethods.Get, pattern, DelegateFactory.Create(_ => handler));
    }

    internal static RouteHandlerBuilder Post(this IEndpointRouteBuilder endpoints, string pattern, Delegate handler)
    {
        return endpoints.MapMethod(HttpMethods.Post, pattern, DelegateFactory.Create(_ => handler));
    }

    internal static RouteHandlerBuilder Options(this IEndpointRouteBuilder endpoints, string pattern, Delegate handler)
    {
        return endpoints.MapMethods(pattern, new[] { HttpMethods.Options }, DelegateFactory.Create(_ => handler));
    }

    private static RouteHandlerBuilder MapMethod(this IEndpointRouteBuilder endpoints, string method, string pattern, Delegate handler)
    {
        DiscoveryEndpoints.Endpoints.Add((pattern, method));

        return endpoints.MapMethods(pattern, new[] { method }, handler);
    }
}
