using FasTnT.Application.Handlers;
using FasTnT.Host.Endpoints.Interfaces;
using FasTnT.Host.Endpoints.Responses.Rest;
using FasTnT.Host.Extensions;

namespace FasTnT.Host.Endpoints;

public static class QueriesEndpoints
{
    public static IEndpointRouteBuilder AddQueriesEndpoints(this IEndpointRouteBuilder app)
    {
        app.Get("queries", ListNamedQueries).RequireAuthorization("query");
        app.Get("queries/{queryName}", GetQueryDefinition).RequireAuthorization("query");
        app.Get("queries/{queryName}/events", GetQueryEvents).RequireAuthorization("query");
        app.Post("queries", CreateNamedQuery).RequireAuthorization("query");
        app.Delete("queries/{queryName}", DeleteNamedQuery).RequireAuthorization("query");

        return app;
    }

    private static async Task<IResult> ListNamedQueries(PaginationContext pagination, QueriesHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListQueriesAsync(pagination.Pagination, cancellationToken);

        return EpcisResults.Ok(new ListCustomQueriesResult(response.Select(x => new CustomQueryDefinitionResult(x.Name, x.Parameters))));
    }

    private static async Task<IResult> GetQueryDefinition(string queryName, QueriesHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.GetQueryDetailsAsync(queryName, cancellationToken);

        return EpcisResults.Ok(new CustomQueryDefinitionResult(response.Name, response.Parameters));
    }

    private static async Task<IResult> GetQueryEvents(string queryName, QueryContext context, QueriesHandler queryHandler, DataRetrieverHandler dataHandler, HttpContext httpContext)
    {
        var query = await queryHandler.GetQueryDetailsAsync(queryName, httpContext.RequestAborted);

        if (httpContext.WebSockets.IsWebSocketRequest)
        {
            return await httpContext.HandleWebsocketAsync(queryName, query.Parameters);
        }
        else
        {
            var response = await dataHandler.QueryEventsAsync(context.Parameters.Union(query.Parameters), httpContext.RequestAborted);

            return EpcisResults.Ok(new QueryResult(new(queryName, response)));
        }
    }

    private static async Task<IResult> CreateNamedQuery(CreateCustomQueryRequest command, QueriesHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.StoreQueryAsync(command.Query, cancellationToken);

        return Results.Created($"queries/{response.Name}", null);
    }

    private static async Task<IResult> DeleteNamedQuery(string queryName, QueriesHandler handler, CancellationToken cancellationToken)
    {
        await handler.DeleteQueryAsync(queryName, cancellationToken);

        return Results.NoContent();
    }
}
