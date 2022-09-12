using FasTnT.Application.Queries.CustomQueries;
using FasTnT.Application.Services.Users;
using FasTnT.Features.v2_0.Endpoints.Interfaces;
using MediatR;

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

    private static async Task<IResult> HandleListNamedQueries(IMediator mediator, CancellationToken cancellationToken)
    {
        var query = new ListCustomQueryDefinition();
        var response = await mediator.Send(query, cancellationToken);

        return IRestResponse.Create(response);
    }

    private static async Task<IResult> HandleGetQueryDefinition(string queryName, IMediator mediator, CancellationToken cancellationToken)
    {
        var query = new GetCustomQueryDefinition(queryName);
        var response = await mediator.Send(query, cancellationToken);

        return IRestResponse.Create(response);
    }

    private static async Task<IResult> HandleGetQueryEvents(string queryName, CancellationToken cancellationToken)
    {
        return Results.Ok();
    }

    private static async Task<IResult> HandleCreateNamedQuery(CancellationToken cancellationToken)
    {
        return Results.Ok();
    }

    private static async Task<IResult> HandleDeleteNamedQuery(string queryName, CancellationToken cancellationToken)
    {
        return Results.Ok();
    }
}

