using FasTnT.Host.Features.v2_0.Endpoints.Interfaces.Utils;

namespace FasTnT.Host.Endpoints.Responses.Rest;

public static class EpcisResults
{
    public static IResult Ok<T>(T result) => new RestResponse<T>(result);
}

