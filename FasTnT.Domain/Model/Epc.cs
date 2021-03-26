using FasTnT.Domain.Enumerations;

namespace FasTnT.Domain.Model
{
    public class Epc
    {
        public Event Event { get; set; }
        public EpcType Type { get; set; }
        public string Id { get; set; }
        public bool IsQuantity { get; set; }
        public float? Quantity { get; set; }
        public string UnitOfMeasure { get; set; }
    }
}
