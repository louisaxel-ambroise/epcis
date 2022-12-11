using FasTnT.Host.Features.v1_2.Communication.Parsers;

namespace FasTnT.Host.Features.v1_2.Endpoints.Interfaces.Utils;

public record SoapEnvelope(string Action, object Query)
{
    public static async ValueTask<SoapEnvelope> BindAsync(HttpContext context)
    {
        var message = await context.Request.ParseSoapEnvelope(context.RequestAborted);

        return new(message.Name.LocalName, XmlQueryParser.Parse(message));
    }
}
