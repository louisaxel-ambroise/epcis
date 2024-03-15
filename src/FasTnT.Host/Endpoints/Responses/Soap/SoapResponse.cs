using FasTnT.Host.Features.v1_2.Communication.Formatters;

namespace FasTnT.Host.Endpoints.Responses.Soap;

public record SoapResponse(object Response) : IResult
{
    public async Task ExecuteAsync(HttpContext context)
    {
        var formattedResponse = XmlResponseFormatter.Format(Response);

        await context.Response.FormatSoap(formattedResponse, context.RequestAborted);
    }
}
