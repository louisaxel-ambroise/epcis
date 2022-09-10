using FasTnT.Domain.Queries.Poll;
using FasTnT.Features.v1_2.Communication.Formatters;
using Microsoft.AspNetCore.Http;

namespace FasTnT.Features.v1_2.Endpoints.Interfaces;

public record SoapResponse(IEpcisResponse Response) : ISoapResponse
{
    public async Task ExecuteAsync(HttpContext context)
    {
        var formattedResponse = XmlResponseFormatter.Format(Response);

        await context.Response.FormatSoap(formattedResponse, context.RequestAborted);
    }
}
