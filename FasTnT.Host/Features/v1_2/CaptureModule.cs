using MediatR;
using FasTnT.Formatter.Xml.Parsers;
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
                    await mediator.Send(request, req.HttpContext.RequestAborted);

                    res.StatusCode = 201;
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
