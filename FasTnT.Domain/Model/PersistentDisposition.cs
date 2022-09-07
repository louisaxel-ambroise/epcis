namespace FasTnT.Domain.Model;

public class PersistentDisposition
{
    public Event Event { get; set; }
    public PersistentDispositionType Type { get; set; }
    public string Id { get; set; }
}
