using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Queries;

namespace FasTnT.Host.Features.v1_2;

public interface ISoapResponse : IResult
{
    public static ISoapResponse Create(IEpcisResponse result) => new SoapResponse(result);
    public static ISoapResponse Fault(EpcisException error) => new SoapFault(error);
}
