using FasTnT.Host.Endpoints;
using FasTnT.Host.Endpoints.Utils;

namespace FasTnT.Host.Extensions;

public static class EndpointRouteHandlerBuilderExtensions
{
    public static RouteHandlerBuilder MapSoap(this IEndpointRouteBuilder app, string route, Action<SoapActionBuilder> soapActions)
    {
        var soapRequest = new SoapActionBuilder();
        soapActions(soapRequest);

        return app.MapPost(route, ErrorHandlingFactory.HandleSoap(soapRequest.SoapAction));
    }

    internal static RouteHandlerBuilder Get(this IEndpointRouteBuilder endpoints, string pattern, Delegate handler)
    {
        return MapMethod(endpoints, HttpMethods.Get, pattern, handler);
    }

    internal static RouteHandlerBuilder Delete(this IEndpointRouteBuilder endpoints, string pattern, Delegate handler)
    {
        return MapMethod(endpoints, HttpMethods.Delete, pattern, handler);
    }

    internal static RouteHandlerBuilder Post(this IEndpointRouteBuilder endpoints, string pattern, Delegate handler)
    {
        return MapMethod(endpoints, HttpMethods.Post, pattern, handler);
    }

    internal static RouteHandlerBuilder Options(this IEndpointRouteBuilder endpoints, string pattern, Delegate handler)
    {
        return MapMethod(endpoints, HttpMethods.Options, pattern, handler);
    }

    private static RouteHandlerBuilder MapMethod(IEndpointRouteBuilder endpoints, string method, string pattern, Delegate handler)
    {
        if (method != HttpMethods.Options)
        {
            DiscoveryEndpoints.Endpoints.Add((pattern, method));
        }

        return endpoints.MapMethods(pattern, new[] { method }, DelegateFactory.Create(_ => handler));
    }
}