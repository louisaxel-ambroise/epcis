using FasTnT.Application.Handlers;
using FasTnT.Domain.Model.Queries;
using FasTnT.Host.Endpoints.Interfaces;
using FasTnT.Host.Endpoints.Responses.Rest;
using FasTnT.Host.Extensions;

namespace FasTnT.Host.Endpoints;

public static class EventsEndpoints
{
    public static IEndpointRouteBuilder AddEventsEndpoints(this IEndpointRouteBuilder app)
    {
        app.Get("events", EventQuery).RequireAuthorization("query");
        app.Get("events/{*eventId}", SingleEventQuery).RequireAuthorization("query");
        app.Get("eventTypes/{eventType}/events", EventTypeQuery).RequireAuthorization("query");
        app.Get("epcs/{epc}/events", EpcQuery).RequireAuthorization("query");
        app.Get("bizSteps/{bizStep}/events", BizStepQuery).RequireAuthorization("query");
        app.Get("bizLocations/{bizLocation}/events", BizLocationQuery).RequireAuthorization("query");
        app.Get("readPoints/{readPoint}/events", ReadPointQuery).RequireAuthorization("query");
        app.Get("dispositions/{disposition}/events", DispositionQuery).RequireAuthorization("query");

        return app;
    }

    private static Task<IResult> EventQuery(QueryContext parameters, DataRetrieverHandler handler, CancellationToken cancellationToken)
    {
        return ExecuteQuery(handler, parameters.Parameters, cancellationToken);
    }

    private static Task<IResult> SingleEventQuery(string eventId, DataRetrieverHandler handler, CancellationToken cancellationToken)
    {
        var parameter = QueryParameter.Create("EQ_eventID", eventId);

        return ExecuteQuery(handler, [parameter], cancellationToken);
    }

    private static Task<IResult> EventTypeQuery(string eventType, QueryContext context, DataRetrieverHandler handler, CancellationToken cancellationToken)
    {
        var parameters = context.Parameters.Append(QueryParameter.Create("eventType", eventType));

        return ExecuteQuery(handler, parameters, cancellationToken);
    }

    private static Task<IResult> EpcQuery(string epc, QueryContext queryParams, DataRetrieverHandler handler, CancellationToken cancellationToken)
    {
        var parameters = queryParams.Parameters.Append(QueryParameter.Create("MATCH_anyEPC", epc));

        return ExecuteQuery(handler, parameters, cancellationToken);
    }

    private static Task<IResult> BizStepQuery(string bizStep, QueryContext context, DataRetrieverHandler handler, CancellationToken cancellationToken)
    {
        var parameters = context.Parameters.Append(QueryParameter.Create("EQ_bizStep", bizStep));

        return ExecuteQuery(handler, parameters, cancellationToken);
    }

    private static Task<IResult> BizLocationQuery(string bizLocation, QueryContext context, DataRetrieverHandler handler, CancellationToken cancellationToken)
    {
        var parameters = context.Parameters.Append(QueryParameter.Create("EQ_bizLocation", bizLocation));

        return ExecuteQuery(handler, parameters, cancellationToken);
    }

    private static Task<IResult> ReadPointQuery(string readPoint, QueryContext context, DataRetrieverHandler handler, CancellationToken cancellationToken)
    {
        var parameters = context.Parameters.Append(QueryParameter.Create("EQ_readPoint", readPoint));

        return ExecuteQuery(handler, parameters, cancellationToken);
    }

    private static Task<IResult> DispositionQuery(string disposition, QueryContext context, DataRetrieverHandler handler, CancellationToken cancellationToken)
    {
        var parameters = context.Parameters.Append(QueryParameter.Create("EQ_disposition", disposition));

        return ExecuteQuery(handler, parameters, cancellationToken);
    }

    private static async Task<IResult> ExecuteQuery(DataRetrieverHandler handler, IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken)
    {
        var response = await handler.QueryEventsAsync(parameters, cancellationToken);

        return EpcisResults.Ok(new QueryResult(new("SimpleEventQuery", response)));
    }
}
