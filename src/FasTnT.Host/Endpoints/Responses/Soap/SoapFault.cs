using FasTnT.Domain.Exceptions;
using FasTnT.Host.Communication.Xml.Formatters;

namespace FasTnT.Host.Endpoints.Responses.Soap;

public record SoapFault(EpcisException Fault) : IResult
{
    public async Task ExecuteAsync(HttpContext context)
    {
        var formattedResponse = XmlResponseFormatter.FormatError(Fault);

        await context.Response.FormatSoap(formattedResponse, context.RequestAborted);
    }
}