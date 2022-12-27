using FasTnT.Application.UseCases.TopLevelResources;
using FasTnT.Host.Features.v2_0.Endpoints.Interfaces;
using FasTnT.Host.Features.v2_0.Endpoints.Interfaces.Utils;

namespace FasTnT.Host.Features.v2_0.Endpoints;

public static class TopLevelEndpoints
{
    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.Get("v2_0/eventTypes", ListEventTypes).RequireAuthorization("query");
        app.Get("v2_0/epcs", ListEpcs).RequireAuthorization("query");
        app.Get("v2_0/bizSteps", ListBizSteps).RequireAuthorization("query");
        app.Get("v2_0/bizLocations", ListBizLocations).RequireAuthorization("query");
        app.Get("v2_0/readPoints", ListReadPoints).RequireAuthorization("query");
        app.Get("v2_0/dispositions", ListDispositions).RequireAuthorization("query");
        app.Get("v2_0/eventTypes/{eventType}", SubResourceRequest).RequireAuthorization("query");
        app.Get("v2_0/epcs/{epc}", SubResourceRequest).RequireAuthorization("query");
        app.Get("v2_0/bizSteps/{bizStep}", SubResourceRequest).RequireAuthorization("query");
        app.Get("v2_0/bizLocations/{bizLocation}", SubResourceRequest).RequireAuthorization("query");
        app.Get("v2_0/readPoints/{readPoint}", SubResourceRequest).RequireAuthorization("query");
        app.Get("v2_0/dispositions/{disposition}", SubResourceRequest).RequireAuthorization("query");

        return app;
    }

    private static async Task<IResult> ListEventTypes(PaginationContext context, IListEventTypes handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListEventTypes(context.Pagination, cancellationToken);

        return EpcisResults.Ok(new CollectionResult(response));
    }

    private static async Task<IResult> ListEpcs(PaginationContext context, IListEpcs handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListEpcs(context.Pagination, cancellationToken);

        return EpcisResults.Ok(new CollectionResult(response));
    }

    private static async Task<IResult> ListBizSteps(PaginationContext context, IListBizSteps handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListBizSteps(context.Pagination, cancellationToken);

        return EpcisResults.Ok(new CollectionResult(response));
    }

    private static async Task<IResult> ListBizLocations(PaginationContext context, IListBizLocations handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListBizLocations(context.Pagination, cancellationToken);

        return EpcisResults.Ok(new CollectionResult(response));
    }

    private static async Task<IResult> ListReadPoints(PaginationContext context, IListReadPoints handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListReadPoints(context.Pagination, cancellationToken);

        return EpcisResults.Ok(new CollectionResult(response));
    }

    private static async Task<IResult> ListDispositions(PaginationContext context, IListDispositions handler, CancellationToken cancellationToken)
    {
        var response = await handler.ListDispositions(context.Pagination, cancellationToken);

        return EpcisResults.Ok(new CollectionResult(response));
    }

    private static IResult SubResourceRequest()
    {
        return EpcisResults.Ok(new CollectionResult(new[] { "events" }));
    }
}
