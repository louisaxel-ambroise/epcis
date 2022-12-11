namespace FasTnT.Domain.Model.Subscriptions;

public class SubscriptionExecutionRecord
{
    public int SubscriptionId { get; set; }
    public DateTime ExecutionTime { get; set; }
    public bool Successful { get; set; }
    public bool ResultsSent { get; set; }
    public string Reason { get; set; }
}
