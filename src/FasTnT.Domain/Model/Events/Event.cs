namespace FasTnT.Domain.Model.Events;

public class Event
{
    public int Id { get; set; }
    public Request Request { get; set; }
    public DateTime EventTime { get; set; }
    public DateTime CaptureTime { get; set; }
    public TimeZoneOffset EventTimeZoneOffset { get; set; } = new();
    public EventType Type { get; set; }
    public EventAction Action { get; set; }
    public string EventId { get; set; }
    public string CertificationInfo { get; set; }
    public string ReadPoint { get; set; }
    public string BusinessLocation { get; set; }
    public string BusinessStep { get; set; }
    public string Disposition { get; set; }
    public string TransformationId { get; set; }
    public DateTime? CorrectiveDeclarationTime { get; set; }
    public string CorrectiveReason { get; set; }
    public List<CorrectiveEventId> CorrectiveEventIds { get; set; } = new();
    public List<Epc> Epcs { get; set; } = new();
    public List<BusinessTransaction> Transactions { get; set; } = new();
    public List<Source> Sources { get; set; } = new();
    public List<Destination> Destinations { get; set; } = new();
    public List<SensorElement> SensorElements { get; set; } = new();
    public List<SensorReport> Reports { get; set; } = new();
    public List<PersistentDisposition> PersistentDispositions { get; set; } = new();
    public List<Field> Fields { get; set; } = new();
}