namespace FasTnT.Domain.Model.Events;

public class BusinessTransaction
{
    public Event Event { get; set; }
    public string Type { get; set; }
    public string Id { get; set; }
}
