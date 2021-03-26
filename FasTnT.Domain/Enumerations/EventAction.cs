using FasTnT.Domain.Utils;

namespace FasTnT.Domain.Enumerations
{
    public class EventAction : Enumeration
    {
        public static readonly EventAction Add = new EventAction(0, "ADD");
        public static readonly EventAction Observe = new EventAction(1, "OBSERVE");
        public static readonly EventAction Delete = new EventAction(2, "DELETE");

        public EventAction() { }
        private EventAction(short id, string displayName) : base(id, displayName) { }
    }
}
