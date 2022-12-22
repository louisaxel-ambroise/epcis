using System.Reflection;
using FasTnT.Domain;
using FasTnT.Host.Features.v1_2.Endpoints.Interfaces;
using FasTnT.Host.Features.v1_2.Extensions;
using FasTnT.Application.UseCases.DataSources;

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
        action.On<PollEvents>(HandlePollEvents);
        action.On<PollMasterData>(HandlePollMasterData);

        return action;
    }

    private static async Task<PollResult> HandlePollEvents(PollEvents query, IDataRetrieveHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.QueryEventsAsync(query.Parameters, cancellationToken);

        return new(new ("SimpleEventQuery", response));
    }

    private static async Task<PollResult> HandlePollMasterData(PollMasterData query, IDataRetrieveHandler handler, CancellationToken cancellationToken)
    {
        var response = await handler.QueryMasterDataAsync(query.Parameters, cancellationToken);

        return new(new ("SimpleMasterDataQuery", response));
    }

    private static Task<GetQueryNamesResult> HandleGetQueryNamesQuery(CancellationToken cancellationToken)
    {
        return Task.FromResult(new GetQueryNamesResult(new[] { "SimpleEventQuery", "SimpleMasterDataQuery" }));
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
