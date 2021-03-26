using FasTnT.Domain.Enumerations;

namespace FasTnT.Domain.Model
{
    public class SourceDestination
    {
        public Event Event { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
        public SourceDestinationType Direction { get; set; }
    }
}
