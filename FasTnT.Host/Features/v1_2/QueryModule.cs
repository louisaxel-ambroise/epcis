using FasTnT.Formatter.Xml;
using MediatR;
using FasTnT.Domain.Exceptions;
using FasTnT.Host.Extensions;
using System.Reflection;
using FasTnT.Application.Queries.Poll;
using FasTnT.Application.Queries.GetStandardVersion;

namespace FasTnT.Host.Features.v1_2
{
    public class QueryModule : Epcis12Module
    {
        public QueryModule(IMediator mediator)
        {
            Get("query.svc", async (req, res) =>
            {
                res.ContentType = "text/xml";
                await Assembly.GetExecutingAssembly()
                              .GetManifestResourceStream(@"FasTnT.Host.Features.v1_2.Artifacts.epcis1_2.wsdl")
                              .CopyToAsync(res.Body)
                              .ConfigureAwait(false);
            });

            Post("query.svc", async (req, res) =>
            {
                res.ContentType = "application/xml";
                
                try
                {
                    var query = await req.ParseSoapEnvelope(req.HttpContext.RequestAborted);
                    var response = query switch
                    {
                        PollQuery poll => XmlResponseFormatter.FormatPoll(await mediator.Send(poll)),
                        GetVendorVersionQuery getVendorVersion => XmlResponseFormatter.FormatVendorVersion(await mediator.Send(getVendorVersion)),
                        GetStandardVersionQuery getStandardVersion => XmlResponseFormatter.FormatStandardVersion(await mediator.Send(getStandardVersion)),
                        // TODO: subscription queries
                        _ => throw new EpcisException(ExceptionType.ValidationException, $"Invalid query: {query.GetType().Name}");
                    };

                    await res.FormatSoap(response, req.HttpContext.RequestAborted);
                }
                catch(EpcisException ex)
                {
                    await res.FormatSoap(XmlResponseFormatter.FormatError(ex), req.HttpContext.RequestAborted);
                }
            });
        }

    }
}
