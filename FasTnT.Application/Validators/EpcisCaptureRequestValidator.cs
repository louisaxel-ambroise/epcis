using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Events;

namespace FasTnT.Application.Validators;

public static class EpcisCaptureRequestValidator
{
    public static bool IsValid(Request request)
    {
        return HaveEventOrMasterdataOrBeACallback(request)
            && request.Events.All(evt => !IsAddOrDeleteAggregation(evt) || HaveAParentIdEpc(evt));
    }

    private static bool HaveEventOrMasterdataOrBeACallback(Request request)
    {
        return request.Events.Count + request.Masterdata.Count > 0
            || request.SubscriptionCallback != null;
    }

    private static bool IsAddOrDeleteAggregation(Event evt)
    {
        return evt.Type == EventType.AggregationEvent && (evt.Action == EventAction.Add || evt.Action == EventAction.Delete);
    }

    private static bool HaveAParentIdEpc(Event evt)
    {
        return evt.Epcs.Any(epc => epc.Type == EpcType.ParentId);
    }
}
