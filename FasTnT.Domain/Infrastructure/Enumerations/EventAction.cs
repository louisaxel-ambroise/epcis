using FasTnT.Domain.Utils;

namespace FasTnT.Domain.Enumerations
{
    public class EventAction : Enumeration
    {
        public static readonly EventAction Add = new(0, "ADD");
        public static readonly EventAction Observe = new(1, "OBSERVE");
        public static readonly EventAction Delete = new(2, "DELETE");

        public EventAction() { }
        private EventAction(short id, string displayName) : base(id, displayName) { }
    }
}
