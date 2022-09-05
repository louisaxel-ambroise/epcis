using FasTnT.Domain.Exceptions;
using FasTnT.Formatter.Xml.Formatters;
using FasTnT.Host.Extensions;

namespace FasTnT.Host.Features.v1_2;

public record SoapFault(EpcisException Fault) : ISoapResponse
{
    public async Task ExecuteAsync(HttpContext context)
    {
        var formattedResponse = XmlResponseFormatter.FormatError(Fault);

        await context.Response.FormatSoap(formattedResponse, context.RequestAborted);
    }
}