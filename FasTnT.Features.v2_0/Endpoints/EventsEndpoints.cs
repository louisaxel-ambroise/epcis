using FasTnT.Application.Services.Users;
using FasTnT.Application.UseCases.ExecuteStandardQuery;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Queries;
using FasTnT.Features.v2_0.Endpoints.Interfaces;
using FasTnT.Features.v2_0.Endpoints.Interfaces.Utils;

namespace FasTnT.Features.v2_0.Endpoints;

public class EventsEndpoints
{
    protected EventsEndpoints() { }

    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("v2_0/events", HandleEventQuery).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));
        app.MapGet("v2_0/events/{eventId}", HandleSingleEventQuery).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));
        app.MapGet("v2_0/eventTypes/{eventType}/events", HandleEventTypeQuery).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));
        app.MapGet("v2_0/epcs/{epc}/events", HandleEpcQuery).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));
        app.MapGet("v2_0/bizSteps/{bizStep}/events", HandleBizStepQuery).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));
        app.MapGet("v2_0/bizLocations/{bizLocation}/events", HandleBizLocationQuery).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));
        app.MapGet("v2_0/readPoints/{readPoint}/events", HandleReadPointQuery).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));
        app.MapGet("v2_0/dispositions/{disposition}/events", HandleDispositionQuery).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));

        return app;
    }

    private static Task<IResult> HandleEventQuery(QueryParameters parameters, IExecuteStandardQueryHandler handler, CancellationToken cancellationToken)
    {
        return ExecuteQuery(handler, parameters.Parameters, cancellationToken);
    }

    private static Task<IResult> HandleSingleEventQuery(string eventId, IExecuteStandardQueryHandler handler, CancellationToken cancellationToken)
    {
        var parameter = QueryParameter.Create("EQ_eventID", new[] { eventId });

        return ExecuteQuery(handler, new[] { parameter }, cancellationToken);
    }

    private static Task<IResult> HandleEventTypeQuery(string eventType, QueryParameters queryParams, IExecuteStandardQueryHandler handler, CancellationToken cancellationToken)
    {
        var parameters = queryParams.Parameters.Append(QueryParameter.Create("eventType", new[] { eventType }));

        return ExecuteQuery(handler, parameters, cancellationToken);
    }

    private static Task<IResult> HandleEpcQuery(string epc, QueryParameters queryParams, IExecuteStandardQueryHandler handler, CancellationToken cancellationToken)
    {
        var parameters = queryParams.Parameters.Append(QueryParameter.Create("MATCH_anyEPC", new[] { epc }));

        return ExecuteQuery(handler, parameters, cancellationToken);
    }

    private static Task<IResult> HandleBizStepQuery(string bizStep, QueryParameters queryParams, IExecuteStandardQueryHandler handler, CancellationToken cancellationToken)
    {
        var parameters = queryParams.Parameters.Append(QueryParameter.Create("EQ_bizStep", new[] { bizStep }));

        return ExecuteQuery(handler, parameters, cancellationToken);
    }

    private static Task<IResult> HandleBizLocationQuery(string bizLocation, QueryParameters queryParams, IExecuteStandardQueryHandler handler, CancellationToken cancellationToken)
    {
        var parameters = queryParams.Parameters.Append(QueryParameter.Create("EQ_bizLocation", new[] { bizLocation }));

        return ExecuteQuery(handler, parameters, cancellationToken);
    }

    private static Task<IResult> HandleReadPointQuery(string readPoint, QueryParameters queryParams, IExecuteStandardQueryHandler handler, CancellationToken cancellationToken)
    {
        var parameters = queryParams.Parameters.Append(QueryParameter.Create("EQ_readPoint", new[] { readPoint }));

        return ExecuteQuery(handler, parameters, cancellationToken);
    }

    private static Task<IResult> HandleDispositionQuery(string disposition, QueryParameters queryParams, IExecuteStandardQueryHandler handler, CancellationToken cancellationToken)
    {
        var parameters = queryParams.Parameters.Append(QueryParameter.Create("EQ_disposition", new[] { disposition }));

        return ExecuteQuery(handler, parameters, cancellationToken);
    }

    private static async Task<IResult> ExecuteQuery(IExecuteStandardQueryHandler handler, IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken)
    {
        try
        {
            var response = await handler.ExecuteQueryAsync("SimpleEventQuery", parameters, cancellationToken);

            return EpcisResults.Ok(response);
        }
        catch (EpcisException ex)
        {
            return EpcisResults.Error(ex);
        }
    }
}

