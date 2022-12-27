namespace FasTnT.Host.Features.v1_2.Extensions;

public static class EndpointRouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder MapSoap(this IEndpointRouteBuilder app, string route, Action<SoapActionBuilder> soapActions)
    {
        var soapRequest = new SoapActionBuilder();
        soapActions(soapRequest);

        return app.MapPost(route, soapRequest.SoapAction);
    }
}