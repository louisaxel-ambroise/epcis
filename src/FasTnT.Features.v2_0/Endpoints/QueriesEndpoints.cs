using FasTnT.Application.UseCases.Queries;
using FasTnT.Domain.Model.Queries;
using FasTnT.Features.v2_0.Endpoints.Interfaces;
using FasTnT.Features.v2_0.Endpoints.Interfaces.Utils;
using FasTnT.Features.v2_0.Subscriptions;

namespace FasTnT.Features.v2_0.Endpoints;

public static class QueriesEndpoints
{
    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.Get("v2_0/queries", HandleListNamedQueries).RequireAuthorization("query");
        app.Get("v2_0/queries/{queryName}", HandleGetQueryDefinition).RequireAuthorization("query");
        app.Get("v2_0/queries/{queryName}/events", ctx => ctx.WebSockets.IsWebSocketRequest ? WebSocketSubscription.SubscribeAsync : HandleGetQueryEvents).RequireAuthorization("query");
        app.Post("v2_0/queries", HandleCreateNamedQuery).RequireAuthorization("query");
        app.MapDelete("v2_0/queries/{queryName}", HandleDeleteNamedQuery).RequireAuthorization("query");

        return app;
    }

    private static async Task<IResult> HandleListNamedQueries(PaginationContext pagination, IListQueriesHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListQueriesAsync(pagination.Pagination, cancellationToken);

        return EpcisResults.Ok(new ListCustomQueriesResult(response.Select(x => new CustomQueryDefinitionResult(x.Name, x.Parameters))));
    }

    private static async Task<IResult> HandleGetQueryDefinition(string queryName, IGetQueryDetailsHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.GetQueryDetailsAsync(queryName, cancellationToken);

        return EpcisResults.Ok(new CustomQueryDefinitionResult(response.Name, response.Parameters));
    }

    private static async Task<IResult> HandleGetQueryEvents(string queryName, QueryContext context, IExecuteQueryHandler queryHandler, CancellationToken cancellationToken)
    {
        var response = await queryHandler.ExecuteQueryAsync(queryName, context.Parameters, cancellationToken);

        return EpcisResults.Ok(new QueryResult(response));
    }

    private static async Task<IResult> HandleCreateNamedQuery(CreateCustomQueryRequest command, IStoreQueryHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.StoreQueryAsync(command.Query, cancellationToken);

        return Results.Created($"v2_0/queries/{response.Name}", null);
    }

    private static async Task<IResult> HandleDeleteNamedQuery(string queryName, IDeleteQueryHandler handler, CancellationToken cancellationToken)
    {
        await handler.DeleteQueryAsync(queryName, cancellationToken);

        return Results.NoContent();
    }
}
