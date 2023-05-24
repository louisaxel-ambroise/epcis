using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Events;

namespace FasTnT.Application.Services.Subscriptions;

public record SubscriptionResult(bool Successful, List<Event> Events, List<int> RequestIds, EpcisException Exception)
{
    public static SubscriptionResult Success(List<Event> events, List<int> requestIds)
    {
        return new(true, events, requestIds, null);
    }

    public static SubscriptionResult Failed(EpcisException error)
    {
        return new(false, null, null, error);
    }
}
