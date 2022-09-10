using FasTnT.Domain.Queries;
using FasTnT.Formatter.Xml.Formatters;
using FasTnT.Host.Extensions;
using Microsoft.AspNetCore.Http;

namespace FasTnT.Host.Features.v1_2;

public record SoapResponse(IEpcisResponse Response) : ISoapResponse
{
    public async Task ExecuteAsync(HttpContext context)
    {
        var formattedResponse = XmlResponseFormatter.Format(Response);

        await context.Response.FormatSoap(formattedResponse, context.RequestAborted);
    }
}
