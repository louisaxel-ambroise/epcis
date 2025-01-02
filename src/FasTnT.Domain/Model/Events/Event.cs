namespace FasTnT.Domain.Model.Events;

public class Event
{
    public int Id { get; set; }
    public Request Request { get; set; }
    public DateTime EventTime { get; set; }
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
    public List<CorrectiveEventId> CorrectiveEventIds { get; set; } = [];
    public List<Epc> Epcs { get; set; } = [];
    public List<BusinessTransaction> Transactions { get; set; } = [];
    public List<Source> Sources { get; set; } = [];
    public List<Destination> Destinations { get; set; } = [];
    public List<SensorElement> SensorElements { get; set; } = [];
    public List<SensorReport> Reports { get; set; } = [];
    public List<PersistentDisposition> PersistentDispositions { get; set; } = [];
    public List<Field> Fields { get; set; } = [];
}