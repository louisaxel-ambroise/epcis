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
    private readonly ILogger<Epcis1_2Module> _logger;

    public Epcis1_2Module(ILogger<Epcis1_2Module> logger) => _logger = logger;

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("v1_2/capture", async (IMediator mediator, HttpRequest req, HttpResponse res) =>
        {
            try
            {
                var request = await CaptureRequestParser.ParseAsync(req.Body, req.HttpContext.RequestAborted);

                _logger.LogInformation("Start capture request processing");
                await mediator.Send(request, req.HttpContext.RequestAborted);

                _logger.LogInformation("Successfully captured request. Id = {requestId}", request.Request.Id);

                res.StatusCode = 201;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to process capture request");

                res.StatusCode = (ex is FormatException or EpcisException)
                    ? 400
                    : 500;
            }
        }).RequireAuthorization(policyNames: "Capture");

        app.MapGet("v1_2/query.svc", async (HttpResponse res) =>
        {
            res.ContentType = "text/xml";

            _logger.LogInformation("Return Query 1.2 WSDL from GET request");

            await using var wsdl = Assembly.GetExecutingAssembly().GetManifestResourceStream(WsdlPath);
            await wsdl.CopyToAsync(res.Body).ConfigureAwait(false);
        }).AllowAnonymous();

        app.MapPost("v1_2/query.svc", async (IMediator mediator, HttpRequest req, HttpResponse res) =>
        {
            var response = default(XElement);

            try
            {
                var queryElement = await req.ParseSoapEnvelope(req.HttpContext.RequestAborted);
                var query = XmlQueryParser.Parse(queryElement);

                _logger.LogInformation("Start processing {queryName} query request", query.GetType().Name);

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
                _logger.LogInformation(ex, "Unable to process query");

                response = SoapExtensions.FormatFault(ex is EpcisException epcisEx ? epcisEx : EpcisException.Default);
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
                _logger.LogInformation("Trigger subscription executions: {triggers}", string.Join(", ", value));

                await mediator.Publish(new TriggerSubscriptionNotification(value.ToArray()));

                res.StatusCode = 202;
            }
            else
            {
                _logger.LogWarning("No trigger values specified.");

                res.StatusCode = 400;
            }
        }).RequireAuthorization(policyNames: "Query");
    }
}
