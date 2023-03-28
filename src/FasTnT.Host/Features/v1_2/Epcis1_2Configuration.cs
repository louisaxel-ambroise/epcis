using FasTnT.Host.Features.v1_2.Endpoints;
using FasTnT.Host.Features.v1_2.Extensions;

namespace FasTnT.Host.Features.v1_2;

public static class Epcis1_2Configuration
{
    public static WebApplication UseEpcis12Endpoints(this WebApplication app)
    {
        CaptureEndpoints.AddRoutes(app);
        QueryEndpoints.AddRoutes(app);
        SubscriptionEndpoints.AddRoutes(app);

        app.MapSoap("v1_2/query.svc", action =>
        {
            QueryEndpoints.AddSoapActions(action);
            SubscriptionEndpoints.AddSoapActions(action);
        }).RequireAuthorization("query");

        return app;
    }

    internal static RouteHandlerBuilder TryMapPost(this IEndpointRouteBuilder endpoints, string pattern, Delegate handler)
    {
        return endpoints.MapPost(pattern, ErrorHandlingFactory.HandleError(handler));
    }
}
