using FasTnT.Application.UseCases.TopLevelResources;
using FasTnT.Features.v2_0.Endpoints.Interfaces;
using FasTnT.Features.v2_0.Endpoints.Interfaces.Utils;

namespace FasTnT.Features.v2_0.Endpoints;

public static class TopLevelEndpoints
{
    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.Get("v2_0/eventTypes", HandleListEventTypes).RequireAuthorization("query");
        app.Get("v2_0/epcs", HandleListEpcs).RequireAuthorization("query");
        app.Get("v2_0/bizSteps", HandleListBizSteps).RequireAuthorization("query");
        app.Get("v2_0/bizLocations", HandleListBizLocations).RequireAuthorization("query");
        app.Get("v2_0/readPoints", HandleListReadPoints).RequireAuthorization("query");
        app.Get("v2_0/dispositions", HandleListDispositions).RequireAuthorization("query");
        app.Get("v2_0/eventTypes/{eventType}", HandleSubResourceRequest).RequireAuthorization("query");
        app.Get("v2_0/epcs/{epc}", HandleSubResourceRequest).RequireAuthorization("query");
        app.Get("v2_0/bizSteps/{bizStep}", HandleSubResourceRequest).RequireAuthorization("query");
        app.Get("v2_0/bizLocations/{bizLocation}", HandleSubResourceRequest).RequireAuthorization("query");
        app.Get("v2_0/readPoints/{readPoint}", HandleSubResourceRequest).RequireAuthorization("query");
        app.Get("v2_0/dispositions/{disposition}", HandleSubResourceRequest).RequireAuthorization("query");

        return app;
    }

    private static async Task<IResult> HandleListEventTypes(PaginationContext context, IListEventTypesHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListEventTypes(context.Pagination, cancellationToken);

        return EpcisResults.Ok(new CollectionResult(response));
    }

    private static async Task<IResult> HandleListEpcs(PaginationContext context, IListEpcsHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListEpcs(context.Pagination, cancellationToken);

        return EpcisResults.Ok(new CollectionResult(response));
    }

    private static async Task<IResult> HandleListBizSteps(PaginationContext context, IListBizStepsHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListBizSteps(context.Pagination, cancellationToken);

        return EpcisResults.Ok(new CollectionResult(response));
    }

    private static async Task<IResult> HandleListBizLocations(PaginationContext context, IListBizLocationsHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListBizLocations(context.Pagination, cancellationToken);

        return EpcisResults.Ok(new CollectionResult(response));
    }

    private static async Task<IResult> HandleListReadPoints(PaginationContext context, IListReadPointsHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListReadPoints(context.Pagination, cancellationToken);

        return EpcisResults.Ok(new CollectionResult(response));
    }

    private static async Task<IResult> HandleListDispositions(PaginationContext context, IListDispositionsHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListDispositions(context.Pagination, cancellationToken);

        return EpcisResults.Ok(new CollectionResult(response));
    }

    private static IResult HandleSubResourceRequest()
    {
        return EpcisResults.Ok(new CollectionResult(new[] { "events" }));
    }
}
