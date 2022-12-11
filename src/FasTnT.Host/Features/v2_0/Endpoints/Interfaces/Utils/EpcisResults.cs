namespace FasTnT.Host.Features.v2_0.Endpoints.Interfaces.Utils;

public static class EpcisResults
{
    public static IResult Ok<T>(T result) => new RestResponse<T>(result);
}

