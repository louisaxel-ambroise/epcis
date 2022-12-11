using FasTnT.Application.UseCases.Queries;
using FasTnT.Domain.Model.Queries;
using FasTnT.Host.Features.v2_0.Endpoints.Interfaces;
using FasTnT.Host.Features.v2_0.Endpoints.Interfaces.Utils;

namespace FasTnT.Host.Features.v2_0.Endpoints;

public static class EventsEndpoints
{
    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.Get("v2_0/events", HandleEventQuery).RequireAuthorization("query");
        app.Get("v2_0/events/{*eventId}", HandleSingleEventQuery).RequireAuthorization("query");
        app.Get("v2_0/eventTypes/{eventType}/events", HandleEventTypeQuery).RequireAuthorization("query");
        app.Get("v2_0/epcs/{epc}/events", HandleEpcQuery).RequireAuthorization("query");
        app.Get("v2_0/bizSteps/{bizStep}/events", HandleBizStepQuery).RequireAuthorization("query");
        app.Get("v2_0/bizLocations/{bizLocation}/events", HandleBizLocationQuery).RequireAuthorization("query");
        app.Get("v2_0/readPoints/{readPoint}/events", HandleReadPointQuery).RequireAuthorization("query");
        app.Get("v2_0/dispositions/{disposition}/events", HandleDispositionQuery).RequireAuthorization("query");

        return app;
    }

    private static Task<IResult> HandleEventQuery(QueryContext parameters, IExecuteQueryHandler handler, CancellationToken cancellationToken)
    {
        return ExecuteQuery(handler, parameters.Parameters, cancellationToken);
    }

    private static Task<IResult> HandleSingleEventQuery(string eventId, IExecuteQueryHandler handler, CancellationToken cancellationToken)
    {
        var parameter = QueryParameter.Create("EQ_eventID", eventId);

        return ExecuteQuery(handler, new[] { parameter }, cancellationToken);
    }

    private static Task<IResult> HandleEventTypeQuery(string eventType, QueryContext context, IExecuteQueryHandler handler, CancellationToken cancellationToken)
    {
        var parameters = context.Parameters.Append(QueryParameter.Create("eventType", eventType));

        return ExecuteQuery(handler, parameters, cancellationToken);
    }

    private static Task<IResult> HandleEpcQuery(string epc, QueryContext queryParams, IExecuteQueryHandler handler, CancellationToken cancellationToken)
    {
        var parameters = queryParams.Parameters.Append(QueryParameter.Create("MATCH_anyEPC", epc));

        return ExecuteQuery(handler, parameters, cancellationToken);
    }

    private static Task<IResult> HandleBizStepQuery(string bizStep, QueryContext context, IExecuteQueryHandler handler, CancellationToken cancellationToken)
    {
        var parameters = context.Parameters.Append(QueryParameter.Create("EQ_bizStep", bizStep));

        return ExecuteQuery(handler, parameters, cancellationToken);
    }

    private static Task<IResult> HandleBizLocationQuery(string bizLocation, QueryContext context, IExecuteQueryHandler handler, CancellationToken cancellationToken)
    {
        var parameters = context.Parameters.Append(QueryParameter.Create("EQ_bizLocation", bizLocation));

        return ExecuteQuery(handler, parameters, cancellationToken);
    }

    private static Task<IResult> HandleReadPointQuery(string readPoint, QueryContext context, IExecuteQueryHandler handler, CancellationToken cancellationToken)
    {
        var parameters = context.Parameters.Append(QueryParameter.Create("EQ_readPoint", readPoint));

        return ExecuteQuery(handler, parameters, cancellationToken);
    }

    private static Task<IResult> HandleDispositionQuery(string disposition, QueryContext context, IExecuteQueryHandler handler, CancellationToken cancellationToken)
    {
        var parameters = context.Parameters.Append(QueryParameter.Create("EQ_disposition", disposition));

        return ExecuteQuery(handler, parameters, cancellationToken);
    }

    private static async Task<IResult> ExecuteQuery(IExecuteQueryHandler handler, IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken)
    {
        var response = await handler.ExecuteQueryAsync("SimpleEventQuery", parameters, cancellationToken);

        return EpcisResults.Ok(new QueryResult(response));
    }
}
