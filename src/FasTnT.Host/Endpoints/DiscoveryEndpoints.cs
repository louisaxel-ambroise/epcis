using FasTnT.Host.Endpoints.Interfaces;
using FasTnT.Host.Endpoints.Responses.Rest;
using FasTnT.Host.Extensions;

namespace FasTnT.Host.Endpoints;

public static class DiscoveryEndpoints
{
    public static readonly List<(string Path, string Method)> Endpoints = [];

    public static IEndpointRouteBuilder AddDiscoveryEndpoints(this IEndpointRouteBuilder app)
    {
        app.Get("", TopLevelResources).RequireAuthorization("query");

        foreach (var path in Endpoints.GroupBy(x => x.Path))
        {
            app.Options(path.Key, Discovery(path.Select(x => x.Method))).AllowAnonymous();
        }

        return app;
    }

    private static Delegate Discovery(IEnumerable<string> methods)
    {
        return (HttpContext ctx) =>
        {
            ctx.Response.Headers.Append("Accept", methods.ToArray());

            return Results.NoContent();
        };
    }

    private static IResult TopLevelResources()
    {
        var resources = Endpoints
            .Select(x => x.Path.Split('/').FirstOrDefault(x => x != "v2_0"))
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct();

        return EpcisResults.Ok(new CollectionResult(resources));
    }
}
