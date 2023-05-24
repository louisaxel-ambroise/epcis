using FasTnT.Domain.Model;

namespace FasTnT.Application.Validators;

public static class RequestValidator
{
    public static bool IsValid(Request request)
    {
        return (ContainsData(request) || IsSubscriptionResult(request)) 
            && request.Events.All(EventValidator.IsValid);
    }

    private static bool ContainsData(Request request)
    {
        return request.Events.Count + request.Masterdata.Count > 0;
    }

    private static bool IsSubscriptionResult(Request request)
    {
        return request.SubscriptionCallback is not null;
    }
}
