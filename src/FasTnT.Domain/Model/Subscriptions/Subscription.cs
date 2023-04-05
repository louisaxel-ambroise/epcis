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
    public List<QueryParameter> Parameters { get; set; } = new();
    public DateTime InitialRecordTime { get; set; }
    public DateTime LastExecutedTime { get; set; }
    public DateTime NextExecutionTime { get; set; }
    public int[] BufferRequestIds { get; set; } = Array.Empty<int>();
}
