using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Queries;

namespace FasTnT.Host.Features.v2_0.Interfaces;

public interface IRestResponse : IResult
{
    public static IRestResponse Create(IEpcisResponse result) => new RestResponse(result);
    public static IRestResponse Fault(EpcisException error) => new RestFault(error);
}

