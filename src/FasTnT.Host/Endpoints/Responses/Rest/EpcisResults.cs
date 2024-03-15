namespace FasTnT.Host.Endpoints.Responses.Rest;

public static class EpcisResults
{
    public static IResult Ok<T>(T result) => new RestResponse<T>(result);
}

