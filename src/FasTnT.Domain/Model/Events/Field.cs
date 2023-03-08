namespace FasTnT.Domain.Model.Events;

public class Field
{
    public FieldType Type { get; set; }
    public string Name { get; set; }
    public string Namespace { get; set; }
    public string TextValue { get; set; }
    public double? NumericValue { get; set; }
    public DateTime? DateValue { get; set; }
    public int Index { get; set; }
    public int? EntityIndex { get; set; }
    public int? ParentIndex { get; set; }
}