using FasTnT.Domain.Queries;
using FasTnT.Formatter.Xml.Parsers;
using FasTnT.Host.Extensions;
using MediatR;

namespace FasTnT.Host.Features.v1_2;

public record SoapEnvelope(IRequest<IEpcisResponse> Query)
{
    public static async ValueTask<SoapEnvelope> BindAsync(HttpContext context)
    {
        var message = await context.Request.ParseSoapEnvelope(context.RequestAborted);

        return new(XmlQueryParser.Parse(message));
    }
}
