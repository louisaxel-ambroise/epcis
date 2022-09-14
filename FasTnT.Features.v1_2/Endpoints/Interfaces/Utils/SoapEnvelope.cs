using FasTnT.Features.v1_2.Communication.Parsers;
using Microsoft.AspNetCore.Http;

namespace FasTnT.Features.v1_2.Endpoints.Interfaces;

public record SoapEnvelope(string Action, object Query)
{
    public static async ValueTask<SoapEnvelope> BindAsync(HttpContext context)
    {
        var message = await context.Request.ParseSoapEnvelope(context.RequestAborted);

        return new(message.Name.LocalName, XmlQueryParser.Parse(message));
    }
}
