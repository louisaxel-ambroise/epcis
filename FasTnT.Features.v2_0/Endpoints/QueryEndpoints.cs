using FasTnT.Application.Services.Users;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Queries.Poll;
using FasTnT.Features.v2_0.Endpoints.Interfaces;
using MediatR;

namespace FasTnT.Features.v2_0.Endpoints;

public class QueryEndpoints
{
    protected QueryEndpoints() { }

    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("v2_0/events", HandleEventQuery).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));
        app.MapGet("v2_0/eventTypes/{eventType}/events", HandleEventTypeQuery).RequireAuthorization(policyNames: nameof(ICurrentUser.CanQuery));

        return app;
    }

    private static Task<IResult> HandleEventQuery(QueryParameters parameters, IMediator mediator, CancellationToken cancellationToken)
    {
        var query = new PollQuery("SimpleEventQuery", parameters.Parameters);

        return HandleFault(mediator, query, cancellationToken);
    }

    private static Task<IResult> HandleEventTypeQuery(string eventType, QueryParameters queryParams, IMediator mediator, CancellationToken cancellationToken)
    {
        var parameters = queryParams.Parameters.Append(new QueryParameter("eventType", new[] { eventType }));
        var query = new PollQuery("SimpleEventQuery", parameters);

        return HandleFault(mediator, query, cancellationToken);
    }

    private static async Task<IResult> HandleFault(IMediator mediator, IEpcisQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var response = await mediator.Send(query, cancellationToken) as PollResponse;

            return IRestResponse.Create(response);
        }
        catch (EpcisException ex)
        {
            return IRestResponse.Fault(ex);
        }
    }
}

