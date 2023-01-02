using FasTnT.Application.UseCases.DataSources;
using FasTnT.Application.UseCases.Queries;
using FasTnT.Host.Features.v2_0.Endpoints.Interfaces;
using FasTnT.Host.Features.v2_0.Endpoints.Interfaces.Utils;
using FasTnT.Host.Features.v2_0.Subscriptions;

namespace FasTnT.Host.Features.v2_0.Endpoints;

public static class QueriesEndpoints
{
    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.Get("v2_0/queries", ListNamedQueries).RequireAuthorization("query");
        app.Get("v2_0/queries/{queryName}", GetQueryDefinition).RequireAuthorization("query");
        app.Get("v2_0/queries/{queryName}/events", ctx => ctx.WebSockets.IsWebSocketRequest ? WebSocketSubscription.SubscribeAsync : GetQueryEvents).RequireAuthorization("query");
        app.Post("v2_0/queries", CreateNamedQuery).RequireAuthorization("query");
        app.MapDelete("v2_0/queries/{queryName}", DeleteNamedQuery).RequireAuthorization("query");

        return app;
    }

    private static async Task<IResult> ListNamedQueries(PaginationContext pagination, IListQueries handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListQueriesAsync(pagination.Pagination, cancellationToken);

        return EpcisResults.Ok(new ListCustomQueriesResult(response.Select(x => new CustomQueryDefinitionResult(x.Name, x.Parameters))));
    }

    private static async Task<IResult> GetQueryDefinition(string queryName, IGetQueryDetails handler, CancellationToken cancellationToken)
    {
        var response = await handler.GetQueryDetailsAsync(queryName, cancellationToken);

        return EpcisResults.Ok(new CustomQueryDefinitionResult(response.Name, response.Parameters));
    }

    private static async Task<IResult> GetQueryEvents(string queryName, QueryContext context, IGetQueryDetails queryDetails, IDataRetriever dataQuery, CancellationToken cancellationToken)
    {
        var query = await queryDetails.GetQueryDetailsAsync(queryName, cancellationToken);
        var response = await dataQuery.QueryEventsAsync(context.Parameters.Union(query.Parameters), cancellationToken);

        return EpcisResults.Ok(new QueryResult(new (queryName, response)));
    }

    private static async Task<IResult> CreateNamedQuery(CreateCustomQueryRequest command, IStoreQuery handler, CancellationToken cancellationToken)
    {
        var response = await handler.StoreQueryAsync(command.Query, cancellationToken);

        return Results.Created($"v2_0/queries/{response.Name}", null);
    }

    private static async Task<IResult> DeleteNamedQuery(string queryName, IDeleteQuery handler, CancellationToken cancellationToken)
    {
        await handler.DeleteQueryAsync(queryName, cancellationToken);

        return Results.NoContent();
    }
}
