using FasTnT.Domain.Enumerations;

namespace FasTnT.Domain.Model.Events;

public class Field
{
    public Event Event { get; set; }
    public SensorElement Element { get; set; }
    public SensorReport Report { get; set; }
    public FieldType Type { get; set; }
    public string Name { get; set; }
    public string Namespace { get; set; }
    public string TextValue { get; set; }
    public double? NumericValue { get; set; }
    public DateTimeOffset? DateValue { get; set; }
    public Field Parent { get; set; }
    public List<Field> Children { get; set; } = new();
}