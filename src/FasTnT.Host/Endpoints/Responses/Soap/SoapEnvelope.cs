using FasTnT.Host.Communication.Xml.Parsers;

namespace FasTnT.Host.Endpoints.Responses.Soap;

public record SoapEnvelope(string Action, Dictionary<string, string> CustomFields, object Query)
{
    public static async ValueTask<SoapEnvelope> BindAsync(HttpContext context)
    {
        var message = await context.Request.ParseSoapEnvelope(context.RequestAborted);

        return SoapQueryParser.Parse(message);
    }
}
