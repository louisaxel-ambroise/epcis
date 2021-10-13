using FasTnT.Domain.Commands.Subscribe;
using FasTnT.Domain.Commands.Unsubscribe;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Notifications;
using FasTnT.Domain.Queries.GetQueryNames;
using FasTnT.Domain.Queries.GetStandardVersion;
using FasTnT.Domain.Queries.GetSubscriptionIds;
using FasTnT.Domain.Queries.GetVendorVersion;
using FasTnT.Domain.Queries.Poll;
using FasTnT.Formatter.Xml.Formatters;
using FasTnT.Formatter.Xml.Parsers;
using FasTnT.Host.Extensions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Primitives;
using System;
using System.Reflection;
using System.Xml.Linq;

namespace FasTnT.Host.Features.v1_2
{
    public class QueryModule : Epcis12Module
    {
        internal const string WsdlPath = "FasTnT.Host.Features.v1_2.Artifacts.epcis1_2.wsdl";

        public QueryModule(IMediator mediator)
        {
            Get("query.svc", async (req, res) =>
            {
                res.ContentType = "text/xml";

                await using var wsdl = Assembly.GetExecutingAssembly().GetManifestResourceStream(WsdlPath);
                await wsdl.CopyToAsync(res.Body).ConfigureAwait(false);
            });

            Post("query.svc", async (req, res) =>
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
                catch(Exception ex)
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

            Get("Trigger", async (req, res) => 
            {
                if(req.Query.TryGetValue("triggers", out StringValues value))
                {
                    await mediator.Publish(new TriggerSubscriptionNotification(value.ToArray()));

                    res.StatusCode = 201;
                }
                else
                {
                    res.StatusCode = 400;
                }
            }).RequireAuthorization(policyNames: "Query");
        }
    }
}
