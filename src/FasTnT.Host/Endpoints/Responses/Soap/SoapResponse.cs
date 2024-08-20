using FasTnT.Host.Communication.Xml.Formatters;

namespace FasTnT.Host.Endpoints.Responses.Soap;

public record SoapResponse(object Response) : IResult
{
    public async Task ExecuteAsync(HttpContext context)
    {
        var formattedResponse = SoapResponseFormatter.Format(Response);

        await context.Response.FormatSoap(formattedResponse, context.RequestAborted);
    }
}
