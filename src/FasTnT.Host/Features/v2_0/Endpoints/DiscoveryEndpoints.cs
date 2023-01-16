using FasTnT.Host.Features.v2_0.Endpoints.Interfaces;
using FasTnT.Host.Features.v2_0.Endpoints.Interfaces.Utils;

namespace FasTnT.Host.Features.v2_0.Endpoints;

public static class DiscoveryEndpoints
{
    public static readonly List<(string Path, string Method)> Endpoints = new();

    public static void AddRoutes(IEndpointRouteBuilder app)
    {
        app.Get("v2_0/", TopLevelResources).RequireAuthorization("query");

        foreach (var path in Endpoints.GroupBy(x => x.Path))
        {
            app.Options(path.Key, Discovery(path.Select(x => x.Method))).AllowAnonymous();
        }
    }

    private static Delegate Discovery(IEnumerable<string> methods)
    {
        return (HttpContext ctx) =>
        {
            ctx.Response.Headers.Add("Accept", methods.ToArray());

            return Results.NoContent();
        };
    }

    private static Task<IResult> TopLevelResources()
    {
        var resources = Endpoints
            .Select(x => x.Path.Split('/').FirstOrDefault(x => x != "v2_0"))
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct();

        return Task.FromResult(EpcisResults.Ok(new CollectionResult(resources)));
    }
}
