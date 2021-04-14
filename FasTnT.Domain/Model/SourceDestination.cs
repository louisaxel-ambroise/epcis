namespace FasTnT.Domain.Model
{
    public class Source
    {
        public Event Event { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
    }
    public class Destination
    {
        public Event Event { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
    }
}
