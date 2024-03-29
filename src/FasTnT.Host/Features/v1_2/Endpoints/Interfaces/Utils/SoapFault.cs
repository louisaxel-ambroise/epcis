﻿using FasTnT.Domain.Exceptions;
using FasTnT.Host.Features.v1_2.Communication.Formatters;

namespace FasTnT.Host.Features.v1_2.Endpoints.Interfaces.Utils;

public record SoapFault(EpcisException Fault) : IResult
{
    public async Task ExecuteAsync(HttpContext context)
    {
        var formattedResponse = XmlResponseFormatter.FormatError(Fault);

        await context.Response.FormatSoap(formattedResponse, context.RequestAborted);
    }
}