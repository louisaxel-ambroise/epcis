using FasTnT.Application.Handlers;
using FasTnT.Domain;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Queries;
using FasTnT.Host.Endpoints.Interfaces;
using FasTnT.Host.Extensions;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace FasTnT.Host.Endpoints;

public static class SoapQueryService
{
    const string WsdlPath = "";

    public static IEndpointRouteBuilder AddSoapQueryService(this IEndpointRouteBuilder app)
    {
        app.MapGet("query.svc", GetWsdl).AllowAnonymous();
        app.MapSoap("query.svc", action =>
        {
            action.Handle(GetQueryNames);
            action.Handle(GetStandardVersion);
            action.Handle(GetVendorVersion);
            action.Handle(Poll);
            action.Handle(GetSubscriptionIDs);
            action.Handle(Subscribe);
            action.Handle(Unsubscribe);
        }).RequireAuthorization("query");

        return app;
    }

    private static async Task<QueryResult> Poll(string queryName, QueryContext query, DataRetrieverHandler handler, CancellationToken cancellationToken)
    {
        QueryResponse response = queryName switch
        {
            "SimpleEventQuery" => new(queryName, await handler.QueryEventsAsync(query.Parameters, cancellationToken)),
            "SimpleMasterdataQuery" => new(queryName, await handler.QueryMasterDataAsync(query.Parameters, cancellationToken)),
            _ => throw new EpcisException(ExceptionType.NoSuchNameException, $"Query '{queryName}' does not exist")
        };

        return new(response);
    }

    private static Task<ListCustomQueriesResult> GetQueryNames()
    {
        var queries = new ListCustomQueriesResult([new("SimpleEventQuery", []), new("SimpleMasterDataQuery", [])]);

        return Task.FromResult(queries);
    }

    private static GetStandardVersionResult GetStandardVersion()
    {
        return new("1.2");
    }

    private static GetVendorVersionResult GetVendorVersion(IOptions<Constants> constants)
    {
        return new(constants.Value.VendorVersion.ToString());
    }

    private static async Task GetWsdl(HttpResponse response, CancellationToken cancellationToken)
    {
        response.ContentType = "text/xml";

        await using var wsdl = Assembly.GetExecutingAssembly().GetManifestResourceStream(WsdlPath);
        await wsdl.CopyToAsync(response.Body, cancellationToken).ConfigureAwait(false);
    }

    private static async Task<ListSubscriptionsResult> GetSubscriptionIDs(SubscriptionsHandler handler, CancellationToken cancellationToken)
    {
        var subscriptions = await handler.ListSubscriptionsAsync("SimpleEventQuery", cancellationToken);

        return new ListSubscriptionsResult(subscriptions);
    }

    private static async Task Subscribe(SubscriptionRequest request, SubscriptionsHandler handler, CancellationToken cancellationToken)
    {
        if (request.Subscription.QueryName != "SimpleEventQuery")
        {
            throw new EpcisException(ExceptionType.SubscribeNotPermittedException, $"Query does not allow subscription: {request.Subscription.QueryName}");
        }

        await handler.RegisterSubscriptionAsync(request.Subscription, cancellationToken);
    }

    private static async Task<UnsubscribeResult> Unsubscribe(Unsubscribe request, SubscriptionsHandler handler, CancellationToken cancellationToken)
    {
        await handler.DeleteSubscriptionAsync(request.SubscriptionId, cancellationToken);

        return new();
    }
}
