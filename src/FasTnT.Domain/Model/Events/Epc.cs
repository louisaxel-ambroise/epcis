namespace FasTnT.Domain.Model.Events;

public class Epc
{
    public EpcType Type { get; set; }
    public string Id { get; set; }
    public float? Quantity { get; set; }
    public string UnitOfMeasure { get; set; }
}
