using Carter;
using FasTnT.Domain.Commands.Subscribe;
using FasTnT.Domain.Commands.Unsubscribe;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Notifications;
using FasTnT.Domain.Queries;
using FasTnT.Formatter.Xml.Formatters;
using FasTnT.Formatter.Xml.Parsers;
using FasTnT.Host.Extensions;
using MediatR;
using Microsoft.Extensions.Primitives;
using System.Reflection;
using System.Xml.Linq;

namespace FasTnT.Host.Features.v1_2;

public class Epcis1_2Module : ICarterModule
{
    internal const string WsdlPath = "FasTnT.Host.Features.v1_2.Artifacts.epcis1_2.wsdl";

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("v1_2/capture", async (IMediator mediator, HttpRequest req, HttpResponse res) =>
        {
            try
            {
                var request = await CaptureRequestParser.ParseAsync(req.Body, req.HttpContext.RequestAborted);
                await mediator.Send(request, req.HttpContext.RequestAborted);

                res.StatusCode = 201;
            }
            catch (Exception ex)
            {
                res.StatusCode = (ex is FormatException or EpcisException)
                    ? 400
                    : 500;
            }
        }).RequireAuthorization(policyNames: "Capture");

        app.MapGet("v1_2/query.svc", async (HttpResponse res) =>
        {
            res.ContentType = "text/xml";

            await using var wsdl = Assembly.GetExecutingAssembly().GetManifestResourceStream(WsdlPath);
            await wsdl.CopyToAsync(res.Body).ConfigureAwait(false);
        });

        app.MapPost("v1_2/query.svc", async (IMediator mediator, HttpRequest req, HttpResponse res) =>
        {
            var response = default(XElement);

            try
            {
                var queryElement = await req.ParseSoapEnvelope(req.HttpContext.RequestAborted);
                var query = XmlQueryParser.Parse(queryElement);

                response = query switch
                {
                    PollQuery poll
                        => XmlResponseFormatter.FormatPoll(await mediator.Send(poll)),
                    GetVendorVersionQuery getVendorVersion
                        => XmlResponseFormatter.FormatVendorVersion(await mediator.Send(getVendorVersion)),
                    GetStandardVersionQuery getStandardVersion
                        => XmlResponseFormatter.FormatStandardVersion(await mediator.Send(getStandardVersion)),
                    GetQueryNamesQuery getQueryNames
                        => XmlResponseFormatter.FormatGetQueryNames(await mediator.Send(getQueryNames)),
                    GetSubscriptionIdsQuery getSubscriptionIds
                        => XmlResponseFormatter.FormatSubscriptionIds(await mediator.Send(getSubscriptionIds)),
                    SubscribeCommand subscribeCommand
                        => XmlResponseFormatter.FormatSubscribeResponse(await mediator.Send(subscribeCommand)),
                    UnsubscribeCommand unsubscribeCommand
                        => XmlResponseFormatter.FormatUnsubscribeResponse(await mediator.Send(unsubscribeCommand)),
                    _
                        => throw new EpcisException(ExceptionType.ValidationException, $"Invalid query: {query.GetType().Name}")
                };
            }
            catch (Exception ex)
            {
                response = ex is EpcisException epcisEx
                    ? XmlResponseFormatter.FormatError(epcisEx)
                    : XmlResponseFormatter.FormatUnexpectedError();
            }
            finally
            {
                await res.FormatSoap(response, req.HttpContext.RequestAborted);
            }
        }).RequireAuthorization(policyNames: "Query");

        app.MapGet("v1_2/Trigger", async (IMediator mediator, HttpRequest req, HttpResponse res) =>
        {
            if (req.Query.TryGetValue("triggers", out StringValues value))
            {
                await mediator.Publish(new TriggerSubscriptionNotification(value.ToArray()));

                res.StatusCode = 202;
            }
            else
            {
                res.StatusCode = 400;
            }
        }).RequireAuthorization(policyNames: "Query");
    }
}
