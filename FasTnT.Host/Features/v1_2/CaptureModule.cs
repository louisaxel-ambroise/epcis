using FasTnT.Formatter.Xml;
using MediatR;
using Microsoft.AspNetCore.Http;
using FasTnT.Formatter.Xml.Parsers;
using System.IO;
using FasTnT.Domain.Exceptions;
using System;

namespace FasTnT.Host.Features.v1_2
{

    public class CaptureModule : Epcis12Module
    {
        public CaptureModule(IMediator mediator)
        {
            Post("capture.svc", async (req, res) =>
            {
                try
                {
                    var request = await CaptureRequestParser.Parse(req.Body, req.HttpContext.RequestAborted);
                    var response = await mediator.Send(request, req.HttpContext.RequestAborted);

                    await res.WriteAsync(XmlResponseFormatter.FormatCaptureResponse(response));
                }
                catch (FormatException)
                {
                    res.StatusCode = 400;
                }
                catch(EpcisException)
                {
                    res.StatusCode = 422;
                }
                catch
                {
                    res.StatusCode =  500;
                }
            });
        }
    }
}
