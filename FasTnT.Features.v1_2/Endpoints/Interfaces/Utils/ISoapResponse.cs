using FasTnT.Domain.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;

namespace FasTnT.Features.v1_2.Endpoints.Interfaces;

public interface ISoapResponse : IResult
{
    public static ISoapResponse Create(object result) => new SoapResponse(result);
    public static ISoapResponse Fault(EpcisException error) => new SoapFault(error);
}
