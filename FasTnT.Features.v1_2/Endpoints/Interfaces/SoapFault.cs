using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Features.v1_2.Communication.Formatters;
using Microsoft.AspNetCore.Http;

namespace FasTnT.Features.v1_2.Endpoints.Interfaces;

public record SoapFault(EpcisException Fault) : ISoapResponse
{
    public async Task ExecuteAsync(HttpContext context)
    {
        var formattedResponse = XmlResponseFormatter.FormatError(Fault);

        await context.Response.FormatSoap(formattedResponse, context.RequestAborted);
    }
}