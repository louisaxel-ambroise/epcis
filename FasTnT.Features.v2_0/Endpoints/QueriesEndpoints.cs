using FasTnT.Application.Services.Users;
using FasTnT.Application.UseCases.CustomQueries;
using FasTnT.Features.v2_0.Endpoints.Interfaces;
using FasTnT.Features.v2_0.Endpoints.Interfaces.Utils;

namespace FasTnT.Features.v2_0.Endpoints;

public class QueriesEndpoints
{
    protected QueriesEndpoints() { }

    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("v2_0/queries", HandleListNamedQueries).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));
        app.MapGet("v2_0/queries/{queryName}", HandleGetQueryDefinition).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));
        app.MapGet("v2_0/queries/{queryName}/events", HandleGetQueryEvents).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));
        app.MapPost("v2_0/queries", HandleCreateNamedQuery).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));
        app.MapDelete("v2_0/queries/{queryName}", HandleDeleteNamedQuery).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));

        return app;
    }

    private static async Task<IResult> HandleListNamedQueries(IListCustomQueriesHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListCustomQueriesAsync(cancellationToken);

        return EpcisResults.Ok(new ListCustomQueriesResult(response.Select(x => new CustomQueryDefinitionResult(x.Name, x.Parameters))));
    }

    private static async Task<IResult> HandleGetQueryDefinition(string queryName, IGetCustomQueryDetailsHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.GetCustomQueryDetailsAsync(queryName, cancellationToken);

        return EpcisResults.Ok(new CustomQueryDefinitionResult(response.Name, response.Parameters));
    }

    private static async Task<IResult> HandleGetQueryEvents(string queryName, IExecuteCustomQueryHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ExecuteQueryAsync(queryName, cancellationToken);

        return EpcisResults.Ok(new QueryResult(response));
    }

    private static async Task<IResult> HandleCreateNamedQuery(CreateCustomQueryRequest command, IStoreCustomQueryHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.StoreQueryAsync(command.Query, cancellationToken);

        return Results.Created($"v2_0/queries/{response.Name}", null);
    }

    private static async Task<IResult> HandleDeleteNamedQuery(string queryName, IDeleteCustomQueryHandler handler, CancellationToken cancellationToken)
    {
        await handler.DeleteQueryAsync(queryName, cancellationToken);

        return Results.NoContent();
    }
}

