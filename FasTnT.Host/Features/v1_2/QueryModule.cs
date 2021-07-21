using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Queries.GetStandardVersion;
using FasTnT.Domain.Queries.Poll;
using FasTnT.Formatter.Xml;
using FasTnT.Host.Extensions;
using MediatR;
using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace FasTnT.Host.Features.v1_2
{
    public class QueryModule : Epcis12Module
    {
        public QueryModule(IMediator mediator)
        {
            Get("query.svc", async (req, res) =>
            {
                res.ContentType = "text/xml";

                using var wsdl = GetWsdlContent();

                await wsdl.CopyToAsync(res.Body)
                          .ConfigureAwait(false);
            });

            Post("query.svc", async (req, res) =>
            {
                var response = default(XElement);

                try
                {
                    var query = await req.ParseSoapEnvelope(req.HttpContext.RequestAborted);

                    response = query switch
                    {
                        PollQuery poll
                            => XmlResponseFormatter.FormatPoll(await mediator.Send(poll)),
                        GetVendorVersionQuery getVendorVersion
                            => XmlResponseFormatter.FormatVendorVersion(await mediator.Send(getVendorVersion)),
                        GetStandardVersionQuery getStandardVersion
                            => XmlResponseFormatter.FormatStandardVersion(await mediator.Send(getStandardVersion)),
                        // TODO: subscription queries
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
            });
        }

        private static Stream GetWsdlContent()
        {
            var wsdlPath = @"FasTnT.Host.Features.v1_2.Artifacts.epcis1_2.wsdl";

            return Assembly.GetExecutingAssembly()
                           .GetManifestResourceStream(wsdlPath);
        }
    }
}
