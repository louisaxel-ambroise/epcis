namespace FasTnT.Domain.Model.Events;

public class SensorElement
{
    public int Index { get; set; }
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
}
