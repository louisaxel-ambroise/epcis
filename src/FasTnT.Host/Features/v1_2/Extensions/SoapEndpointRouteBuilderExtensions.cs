namespace FasTnT.Host.Features.v1_2.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static RouteHandlerBuilder MapSoap(this IEndpointRouteBuilder app, string route, Action<SoapActionBuilder> soapActions)
    {
        var soapRequestHandler = new SoapActionBuilder();
        soapActions(soapRequestHandler);

        return app.MapPost(route, soapRequestHandler.HandleSoapAction);
    }
}