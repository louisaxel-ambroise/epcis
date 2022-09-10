using FasTnT.Domain.Queries.Poll;
using FasTnT.Features.v1_2.Communication.Parsers;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FasTnT.Features.v1_2.Endpoints.Interfaces;

public record SoapEnvelope(IRequest<IEpcisResponse> Query)
{
    public static async ValueTask<SoapEnvelope> BindAsync(HttpContext context)
    {
        var message = await context.Request.ParseSoapEnvelope(context.RequestAborted);

        return new(XmlQueryParser.Parse(message));
    }
}
