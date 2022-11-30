using FasTnT.Features.v2_0.Endpoints.Interfaces;
using FasTnT.Features.v2_0.Endpoints.Interfaces.Utils;

namespace FasTnT.Features.v2_0.Endpoints;

public static class DiscoveryEndpoints
{
    public static readonly List<(string Path, string Method)> Endpoints = new ();

    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.Get("v2_0/", HandleTopLevelResources).RequireAuthorization("query");

        foreach(var path in Endpoints.GroupBy(x => x.Path)) 
        {
            app.Options(path.Key, HandleDiscovery(path.Select(x => x.Method))).AllowAnonymous();
        }

        return app;
    }

    private static Delegate HandleDiscovery(IEnumerable<string> methods)
    {
        return (HttpContext ctx) =>
        {
            ctx.Response.Headers.Add("Accept", methods.ToArray());

            return Results.NoContent();
        };
    }

    private static Task<IResult> HandleTopLevelResources()
    {
        var resources = Endpoints
            .Select(x => x.Path.Split('/').FirstOrDefault(x => x != "v2_0"))
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct();

        return Task.FromResult(EpcisResults.Ok(new CollectionResult(resources)));
    }
}
