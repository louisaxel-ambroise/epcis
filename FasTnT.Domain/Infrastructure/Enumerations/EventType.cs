using FasTnT.Domain.Utils;

namespace FasTnT.Domain.Enumerations
{
    public class EventType : Enumeration
    {
        public static readonly EventType Object = new(0, "ObjectEvent");
        public static readonly EventType Aggregation = new(1, "AggregationEvent");
        public static readonly EventType Transaction = new(2, "TransactionEvent");
        public static readonly EventType Transformation = new(3, "TransformationEvent");
        public static readonly EventType Quantity = new(4, "QuantityEvent");

        public EventType() { }
        public EventType(short id, string displayName) : base(id, displayName) { }
    }
}
