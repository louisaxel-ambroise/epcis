namespace FasTnT.Host.Subscriptions.Schedulers;

public class TriggeredSubscriptionScheduler : SubscriptionScheduler
{
    public override void ComputeNextExecution(DateTime startDate)
    {
        // Trigger every streaming subscription at least every 30 seconds in case a request was missed.
        NextComputedExecution = DateTime.UtcNow.AddSeconds(30);
    }

    public override bool IsDue()
    {
        // A streaming subscription is "always" due, as the request shall be sent to the recipient as soon as possible 
        return true;
    }
}
