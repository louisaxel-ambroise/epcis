using System.Reflection;
using FasTnT.Domain;
using FasTnT.Host.Features.v1_2.Endpoints.Interfaces;
using FasTnT.Host.Features.v1_2.Extensions;
using FasTnT.Application.Handlers;
using Microsoft.Extensions.Options;

namespace FasTnT.Host.Features.v1_2.Endpoints;

public static class QueryEndpoints
{
    internal const string WsdlPath = "FasTnT.Host.Features.v1_2.Artifacts.epcis1_2.wsdl";

    public static IEndpointRouteBuilder AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("v1_2/query.svc", GetWsdl).AllowAnonymous();

        return app;
    }

    public static void AddSoapActions(SoapActionBuilder action)
    {
        action.On<GetQueryNames>(GetQueryNamesQuery);
        action.On<GetStandardVersion>(GetStandardVersionQuery);
        action.On<GetVendorVersion>(GetVendorVersionQuery);
        action.On<PollEvents>(SimpleEventQuery);
        action.On<PollMasterData>(SimpleMasterDataQuery);
    }

    private static async Task<PollResult> SimpleEventQuery(PollEvents query, DataRetrieverHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.QueryEventsAsync(query.Parameters, cancellationToken);

        return new(nameof(SimpleEventQuery), response);
    }

    private static async Task<PollResult> SimpleMasterDataQuery(PollMasterData query, DataRetrieverHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.QueryMasterDataAsync(query.Parameters, cancellationToken);

        return new(nameof(SimpleMasterDataQuery), response);
    }

    private static GetQueryNamesResult GetQueryNamesQuery()
    {
        return new (new[] { nameof(SimpleEventQuery), nameof(SimpleMasterDataQuery) });
    }

    private static GetStandardVersionResult GetStandardVersionQuery()
    {
        return new ("1.2");
    }

    private static GetVendorVersionResult GetVendorVersionQuery(IOptions<Constants> constants)
    {
        return new (constants.Value.VendorVersion.ToString());
    }

    private static async Task GetWsdl(HttpResponse response, CancellationToken cancellationToken)
    {
        response.ContentType = "text/xml";

        await using var wsdl = Assembly.GetExecutingAssembly().GetManifestResourceStream(WsdlPath);
        await wsdl.CopyToAsync(response.Body, cancellationToken).ConfigureAwait(false);
    }
}
