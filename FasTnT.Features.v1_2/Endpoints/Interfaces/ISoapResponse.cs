using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Queries.Poll;
using Microsoft.AspNetCore.Http;

namespace FasTnT.Features.v1_2.Endpoints.Interfaces;

public interface ISoapResponse : IResult
{
    public static ISoapResponse Create(IEpcisResponse result) => new SoapResponse(result);
    public static ISoapResponse Fault(EpcisException error) => new SoapFault(error);
}
