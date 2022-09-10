using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Queries.Poll;

namespace FasTnT.Features.v2_0.Endpoints.Interfaces;

public interface IRestResponse : IResult
{
    public static IRestResponse Create(IEpcisResponse result) => new RestResponse(result);
    public static IRestResponse Fault(EpcisException error) => new RestFault(error);
}

