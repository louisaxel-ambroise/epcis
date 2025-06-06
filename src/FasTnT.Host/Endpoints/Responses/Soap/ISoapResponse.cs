﻿using FasTnT.Domain.Exceptions;

namespace FasTnT.Host.Endpoints.Responses.Soap;

public static class SoapResults
{
    public static IResult FromResult(object result) => new SoapResponse(result);
    public static IResult Fault(EpcisException error) => new SoapFault(error);
}
