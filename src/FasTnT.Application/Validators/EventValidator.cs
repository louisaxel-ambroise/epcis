using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model.Events;

namespace FasTnT.Application.Validators;

public static class EventValidator
{
    public static bool IsValid(Event evt)
    {
        return !IsAddOrDeleteAggregation(evt) || HasParentIdEpc(evt);
    }

    private static bool IsAddOrDeleteAggregation(Event evt)
    {
        return evt.Type == EventType.AggregationEvent && (evt.Action == EventAction.Add || evt.Action == EventAction.Delete);
    }

    private static bool HasParentIdEpc(Event evt)
    {
        return evt.Epcs.Any(epc => epc.Type == EpcType.ParentId);
    }
}