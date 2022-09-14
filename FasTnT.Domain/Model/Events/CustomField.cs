using FasTnT.Domain.Enumerations;

namespace FasTnT.Domain.Model;

public class CustomField
{
    public Event Event { get; set; }
    public FieldType Type { get; set; }
    public string Name { get; set; }
    public string Namespace { get; set; }
    public string TextValue { get; set; }
    public double? NumericValue { get; set; }
    public DateTime? DateValue { get; set; }
    public CustomField Parent { get; set; }
    public bool HasParent { get; set; }
    public List<CustomField> Children { get; set; } = new();
}
