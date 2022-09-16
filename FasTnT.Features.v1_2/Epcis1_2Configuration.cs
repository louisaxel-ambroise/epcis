using FasTnT.Application.Services.Users;
using FasTnT.Features.v1_2.Endpoints;
using FasTnT.Features.v1_2.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace FasTnT.Features.v1_2;

public static class Epcis1_2Configuration
{
    public static IEndpointRouteBuilder MapEpcis12Endpoints(this IEndpointRouteBuilder endpoints)
    {
        CaptureEndpoints.AddRoutes(endpoints);
        QueryEndpoints.AddRoutes(endpoints);
        SubscriptionEndpoints.AddRoutes(endpoints);

        endpoints.MapSoap("v1_2/query.svc", action =>
        {
            QueryEndpoints.AddSoapActions(action);
            SubscriptionEndpoints.AddSoapActions(action);
        }).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));

        return endpoints;
    }
}
