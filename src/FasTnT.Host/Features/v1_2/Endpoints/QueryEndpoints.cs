using System.Reflection;
using FasTnT.Domain;
using FasTnT.Application.UseCases.Queries;
using FasTnT.Domain.Model.Queries;
using FasTnT.Host.Features.v1_2.Endpoints.Interfaces;
using FasTnT.Host.Features.v1_2.Extensions;

namespace FasTnT.Host.Features.v1_2.Endpoints;

public static class QueryEndpoints
{
    internal const string WsdlPath = "FasTnT.Host.Features.v1_2.Artifacts.epcis1_2.wsdl";

    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("v1_2/query.svc", HandleGetWsdl).AllowAnonymous();

        return app;
    }

    public static SoapActionBuilder AddSoapActions(SoapActionBuilder action)
    {
        action.On<GetQueryNames>(HandleGetQueryNamesQuery);
        action.On<GetStandardVersion>(HandleGetStandardVersionQuery);
        action.On<GetVendorVersion>(HandleGetVendorVersionQuery);
        action.On<Poll>(HandlePollQuery);

        return action;
    }

    private static async Task<PollResult> HandlePollQuery(Poll query, IExecuteQueryHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.ExecuteQueryAsync(query.QueryName, query.Parameters, cancellationToken);

        return new(response);
    }

    private static async Task<GetQueryNamesResult> HandleGetQueryNamesQuery(IListQueriesHandler handler, CancellationToken cancellationToken)
    {
        var queries = await handler.ListQueriesAsync(Pagination.Max, cancellationToken);
        var queryNames = queries.Select(x => x.Name);

        return new(queryNames);
    }

    private static Task<GetStandardVersionResult> HandleGetStandardVersionQuery()
    {
        return Task.FromResult(new GetStandardVersionResult("1.2"));
    }

    private static Task<GetVendorVersionResult> HandleGetVendorVersionQuery()
    {
        return Task.FromResult(new GetVendorVersionResult(Constants.Instance.VendorVersion));
    }

    private static async Task HandleGetWsdl(HttpResponse response, CancellationToken cancellationToken)
    {
        response.ContentType = "text/xml";

        await using var wsdl = Assembly.GetExecutingAssembly().GetManifestResourceStream(WsdlPath);
        await wsdl.CopyToAsync(response.Body, cancellationToken).ConfigureAwait(false);
    }
}
