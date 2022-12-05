using FasTnT.Domain.Model.CustomQueries;

namespace FasTnT.Domain.Model.Subscriptions;

public class Subscription
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string QueryName { get; set; }
    public string SignatureToken { get; set; }
    public string FormatterName { get; set; }
    public SubscriptionSchedule Schedule { get; set; }
    public string Trigger { get; set; }
    public bool ReportIfEmpty { get; set; }
    public string Destination { get; set; }
    public StoredQuery Query { get; set; }
    public List<SubscriptionParameter> Parameters { get; set; } = new();
    public List<SubscriptionExecutionRecord> ExecutionRecords { get; set; } = new();
    public DateTimeOffset? InitialRecordTime { get; set; }
}

public class PendingRequest
{
    public int RequestId { get; set; }
    public int SubscriptionId { get; set; }
}
