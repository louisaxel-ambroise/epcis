using FasTnT.Application.Handlers;
using FasTnT.Host.Endpoints.Interfaces;
using FasTnT.Host.Endpoints.Responses.Rest;
using FasTnT.Host.Extensions;

namespace FasTnT.Host.Endpoints;

public static class TopLevelEndpoints
{
    public static IEndpointRouteBuilder AddTopLevelEndpoints(this IEndpointRouteBuilder app)
    {
        app.Get("eventTypes", ListEventTypes).RequireAuthorization("query");
        app.Get("epcs", ListEpcs).RequireAuthorization("query");
        app.Get("bizSteps", ListBizSteps).RequireAuthorization("query");
        app.Get("bizLocations", ListBizLocations).RequireAuthorization("query");
        app.Get("readPoints", ListReadPoints).RequireAuthorization("query");
        app.Get("dispositions", ListDispositions).RequireAuthorization("query");
        app.Get("eventTypes/{eventType}", SubResourceRequest).RequireAuthorization("query");
        app.Get("epcs/{epc}", SubResourceRequest).RequireAuthorization("query");
        app.Get("bizSteps/{bizStep}", SubResourceRequest).RequireAuthorization("query");
        app.Get("bizLocations/{bizLocation}", SubResourceRequest).RequireAuthorization("query");
        app.Get("readPoints/{readPoint}", SubResourceRequest).RequireAuthorization("query");
        app.Get("dispositions/{disposition}", SubResourceRequest).RequireAuthorization("query");

        return app;
    }

    private static IResult ListEventTypes(PaginationContext context)
    {
        var response = TopLevelResourceHandler.ListEventTypes(context.Pagination);

        return EpcisResults.Ok(new CollectionResult(response));
    }

    private static async Task<IResult> ListEpcs(PaginationContext context, TopLevelResourceHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListEpcs(context.Pagination, cancellationToken);

        return EpcisResults.Ok(new CollectionResult(response));
    }

    private static async Task<IResult> ListBizSteps(PaginationContext context, TopLevelResourceHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListBizSteps(context.Pagination, cancellationToken);

        return EpcisResults.Ok(new CollectionResult(response));
    }

    private static async Task<IResult> ListBizLocations(PaginationContext context, TopLevelResourceHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListBizLocations(context.Pagination, cancellationToken);

        return EpcisResults.Ok(new CollectionResult(response));
    }

    private static async Task<IResult> ListReadPoints(PaginationContext context, TopLevelResourceHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListReadPoints(context.Pagination, cancellationToken);

        return EpcisResults.Ok(new CollectionResult(response));
    }

    private static async Task<IResult> ListDispositions(PaginationContext context, TopLevelResourceHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListDispositions(context.Pagination, cancellationToken);

        return EpcisResults.Ok(new CollectionResult(response));
    }

    private static IResult SubResourceRequest()
    {
        return EpcisResults.Ok(new CollectionResult([ "events" ]));
    }
}
