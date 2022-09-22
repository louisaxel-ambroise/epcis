namespace FasTnT.Domain.Model.Events;

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

public class SensorElement
{
    public Event Event { get; set; }
    public DateTime? Time { get; set; }
    public string DeviceId { get; set; }
    public string DeviceMetadata { get; set; }
    public string RawData { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string DataProcessingMethod { get; set; }
    public string BizRules { get; set; }
    public List<SensorReport> Reports { get; set; } = new();
    public List<Field> Fields { get; set; } = new();
}

public class SensorReport
{
    public SensorElement SensorElement { get; set; }
    public string Type { get; set; }
    public string DeviceId { get; set; }
    public string RawData { get; set; }
    public string DataProcessingMethod { get; set; }
    public DateTime? Time { get; set; }
    public string Microorganism { get; set; }
    public string ChemicalSubstance { get; set; }
    public float? Value { get; set; }
    public string Component { get; set; }
    public string StringValue { get; set; }
    public bool BooleanValue { get; set; }
    public string HexBinaryValue { get; set; }
    public string UriValue { get; set; }
    public float? MinValue { get; set; }
    public float? MaxValue { get; set; }
    public float? MeanValue { get; set; }
    public float? PercRank { get; set; }
    public float? PercValue { get; set; }
    public string UnitOfMeasure { get; set; }
    public float? SDev { get; set; }
    public string DeviceMetadata { get; set; }
    public List<Field> Fields { get; set; } = new();
}