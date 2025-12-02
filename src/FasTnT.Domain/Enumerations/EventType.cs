namespace FasTnT.Domain.Enumerations;

public enum EventType
{
    None,
    ObjectEvent,
    AggregationEvent,
    TransactionEvent,
    TransformationEvent,
    QuantityEvent,
    AssociationEvent
}

public static class EnumUtils
{
    public static T Parse<T>(this string value) where T : struct
    {
        return Enum.Parse<T>(value, true);
    }
}