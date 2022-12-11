using FasTnT.Domain.Infrastructure.Exceptions;

namespace FasTnT.Host.Features.v1_2.Endpoints.Interfaces.Utils;

public static class SoapResults
{
    public static IResult Create(object result) => new SoapResponse(result);
    public static IResult Fault(EpcisException error) => new SoapFault(error);
}
