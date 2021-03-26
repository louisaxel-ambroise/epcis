using FasTnT.Domain.Utils;

namespace FasTnT.Domain.Enumerations
{
    public class SourceDestinationType : Enumeration
    {
        public static readonly SourceDestinationType Source = new SourceDestinationType(0, "source");
        public static readonly SourceDestinationType Destination = new SourceDestinationType(1, "destination");

        public SourceDestinationType() { }
        public SourceDestinationType(short id, string displayName) : base(id, displayName) { }
    }
}
