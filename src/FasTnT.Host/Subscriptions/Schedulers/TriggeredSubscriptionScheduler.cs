namespace FasTnT.Host.Subscriptions.Schedulers;

public sealed class TriggeredSubscriptionScheduler : SubscriptionScheduler
{
    // The default delay to run triggered subscriptions in case no even was received
    private static readonly TimeSpan Delay = TimeSpan.FromMinutes(2);

    public override void ComputeNextExecution(DateTime startDate)
    {
        // Trigger every streaming subscription with a regular schedule in case a capture operation was missed or failed.
        NextComputedExecution = DateTime.UtcNow + Delay;
    }

    public override bool IsDue()
    {
        // A streaming subscription is "always" due, as the request shall be sent to the recipient as soon as possible 
        return true;
    }
}
