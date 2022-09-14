using FasTnT.Application.Services.Users;
using FasTnT.Application.UseCases.DeleteCustomQuery;
using FasTnT.Application.UseCases.ExecuteCustomQuery;
using FasTnT.Application.UseCases.GetCustomQueryDetails;
using FasTnT.Application.UseCases.ListCustomQueries;
using FasTnT.Application.UseCases.StoreCustomQuery;
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
        var customQueries = await handler.ListCustomQueriesAsync(cancellationToken);

        return EpcisResults.Ok(new ListCustomQueriesResult(customQueries.Select(x => new CustomQueryDefinitionResult(x.Name, x.Parameters))));
    }

    private static async Task<IResult> HandleGetQueryDefinition(string queryName, IGetCustomQueryDetailsHandler handler, CancellationToken cancellationToken)
    {
        var query = await handler.GetCustomQueryDetailsAsync(queryName, cancellationToken);

        return EpcisResults.Ok(new CustomQueryDefinitionResult(query.Name, query.Parameters));
    }

    private static async Task<IResult> HandleGetQueryEvents(string queryName, IExecuteCustomQueryHandler handler, CancellationToken cancellationToken)
    {
        var queryResponse = await handler.ExecuteQueryAsync(queryName, cancellationToken);

        return EpcisResults.Ok(queryResponse);
    }

    private static async Task<IResult> HandleCreateNamedQuery(CreateCustomQueryRequest command, IStoreCustomQueryHandler handler, CancellationToken cancellationToken)
    {
        await handler.StoreQueryAsync(command.Query, cancellationToken);

        return Results.Created($"v2_0/queries/{command.Query.Name}", null);
    }

    private static async Task<IResult> HandleDeleteNamedQuery(string queryName, IDeleteCustomQueryHandler handler, CancellationToken cancellationToken)
    {
        await handler.DeleteQueryAsync(queryName, cancellationToken);

        return Results.NoContent();
    }
}

