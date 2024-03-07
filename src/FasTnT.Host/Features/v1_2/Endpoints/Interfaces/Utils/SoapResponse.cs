using FasTnT.Host.Communication.Xml.Formatters;

namespace FasTnT.Host.Features.v1_2.Endpoints.Interfaces.Utils;

public record SoapResponse(object Response) : IResult
{
    public async Task ExecuteAsync(HttpContext context)
    {
        var formattedResponse = SoapResponseFormatter.Format(Response);

        await context.Response.FormatSoap(formattedResponse, context.RequestAborted);
    }
}
