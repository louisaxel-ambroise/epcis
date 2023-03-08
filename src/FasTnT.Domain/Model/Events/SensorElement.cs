namespace FasTnT.Domain.Model.Events;

public class SensorElement
{
    public int Index { get; set; }
    public DateTime? Time { get; set; }
    public string DeviceId { get; set; }
    public string DeviceMetadata { get; set; }
    public string RawData { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string DataProcessingMethod { get; set; }
    public string BizRules { get; set; }
}
