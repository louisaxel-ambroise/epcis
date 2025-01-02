using FasTnT.Domain.Model.Queries;

namespace FasTnT.Domain.Model.Subscriptions;

public class Subscription
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string SignatureToken { get; set; }
    public string QueryName { get; set; }
    public string FormatterName { get; set; }
    public SubscriptionSchedule Schedule { get; set; }
    public string Trigger { get; set; }
    public bool ReportIfEmpty { get; set; }
    public string Destination { get; set; }
    public List<QueryParameter> Parameters { get; set; } = [];
    public DateTime InitialRecordTime { get; set; } = DateTime.UtcNow;
    public DateTime LastExecutedTime { get; set; } = DateTime.UtcNow;
    public int[] BufferRequestIds { get; set; } = Array.Empty<int>();
}
