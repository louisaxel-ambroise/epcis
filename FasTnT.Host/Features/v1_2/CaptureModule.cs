using FasTnT.Domain.Exceptions;
using FasTnT.Formatter.Xml.Parsers;
using MediatR;
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
                    await mediator.Send(request, req.HttpContext.RequestAborted);

                    res.StatusCode = 201;
                }
                catch (Exception ex) 
                {
                    res.StatusCode = (ex is FormatException or EpcisException)
                        ? 400
                        : 500;
                }
            });
        }
    }
}
