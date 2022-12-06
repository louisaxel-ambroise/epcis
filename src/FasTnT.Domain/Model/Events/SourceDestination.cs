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
    public DateTimeOffset? Time { get; set; }
    public string DeviceId { get; set; }
    public string DeviceMetadata { get; set; }
    public string RawData { get; set; }
    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }
    public string DataProcessingMethod { get; set; }
    public string BizRules { get; set; }
    public List<SensorReport> Reports { get; set; } = new();
    public List<Field> Fields { get; set; } = new();
}
