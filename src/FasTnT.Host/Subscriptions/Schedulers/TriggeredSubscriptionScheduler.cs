using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Host.Subscriptions.Schedulers;

public class TriggeredSubscriptionScheduler : SubscriptionScheduler
{
    public static readonly SubscriptionSchedule HourlySchedule = new() { Second = "0", Minute = "0" };
    public static readonly SubscriptionSchedule DailySchedule = new() { Second = "0", Minute = "0", Hour = "0" };
    public static readonly SubscriptionSchedule WeeklySchedule = new() { Second = "0", Minute = "0", Hour = "0", DayOfWeek = "1" };
    public static readonly SubscriptionSchedule MonthlySchedule = new() { Second = "0", Minute = "0", Hour = "0", DayOfWeek = "1", DayOfMonth = "1" };

    public static SubscriptionScheduler Create(string trigger)
    {
        return trigger switch
        {
            "hourly" => new CronSubscriptionScheduler(HourlySchedule),
            "daily" => new CronSubscriptionScheduler(DailySchedule),
            "weekly" => new CronSubscriptionScheduler(WeeklySchedule),
            "monthly" => new CronSubscriptionScheduler(MonthlySchedule),
            "stream" => new TriggeredSubscriptionScheduler(),
            _ => throw new EpcisException(ExceptionType.SubscriptionControlsException, "Trigger value is invalid")
        };
    }

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
