using FasTnT.Formatter.Xml;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using FasTnT.Domain.Exceptions;

namespace FasTnT.Host.Features.v1_2
{
    public class QueryModule : Epcis12Module
    {
        public QueryModule(IMediator mediator)
        {
            Get("query.svc", async (req, res) =>
            {
                await res.WriteAsync("WSDL");
            });

            Post("query.svc", async (req, res) =>
            {
                res.ContentType = "application/xml";
                
                try
                {
                    var queryParser = await SoapQueryParser.ParseEnvelop(req.Body, req.HttpContext.RequestAborted);
                    var response = queryParser.Action switch
                    {
                        "Poll" => XmlResponseFormatter.FormatPoll(await mediator.Send(queryParser.ParsePollQuery())),
                        "GetVendorVersion" => XmlResponseFormatter.FormatVendorVersion(await mediator.Send(queryParser.ParseGetVendorVersion())),
                        "GetStandardVersion" => XmlResponseFormatter.FormatStandardVersion(await mediator.Send(queryParser.ParseGetStandardVersion())),
                        _ => throw new Exception()
                    };

                    await res.WriteAsync(response, req.HttpContext.RequestAborted);
                }
                catch(EpcisException ex)
                {
                    await res.WriteAsync(XmlResponseFormatter.FormatError(ex), req.HttpContext.RequestAborted);
                }
            });
        }
    }
}
