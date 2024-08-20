using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Host.Subscriptions.Schedulers;

public abstract class SubscriptionScheduler
{
    public static readonly SubscriptionSchedule
        HourlySchedule = new() { Second = "0", Minute = "0" },
        DailySchedule = new() { Second = "0", Minute = "0", Hour = "0" },
        WeeklySchedule = new() { Second = "0", Minute = "0", Hour = "0", DayOfWeek = "1" },
        MonthlySchedule = new() { Second = "0", Minute = "0", Hour = "0", DayOfWeek = "1", DayOfMonth = "1" };

    public bool Stopped { get; private set; }

    protected DateTime NextComputedExecution;
    protected ManualResetEvent EventHandler = new(false);

    public static SubscriptionScheduler Create(Subscription subscription) => Create(subscription.Trigger, subscription.Schedule);
    public static SubscriptionScheduler Create(string trigger, SubscriptionSchedule schedule)
    {
        return (trigger, schedule) switch
        {
            var _ when trigger == "hourly" => new CronSubscriptionScheduler(HourlySchedule),
            var _ when trigger == "daily" => new CronSubscriptionScheduler(DailySchedule),
            var _ when trigger == "weekly" => new CronSubscriptionScheduler(WeeklySchedule),
            var _ when trigger == "monthly" => new CronSubscriptionScheduler(MonthlySchedule),
            var _ when trigger == "stream" => new TriggeredSubscriptionScheduler(),
            var _ when !IsEmpty(schedule) => new CronSubscriptionScheduler(schedule),
            _ => throw new EpcisException(ExceptionType.SubscriptionControlsException, "Subscription trigger and/or schedule is invalid")
        };
    }

    private static bool IsEmpty(SubscriptionSchedule schedule)
    {
        var parts = new[] { schedule.Second, schedule.Minute, schedule.Hour, schedule.Month, schedule.DayOfWeek, schedule.DayOfMonth };

        return parts.All(string.IsNullOrEmpty);
    }

    public abstract void ComputeNextExecution(DateTime startDate);

    public virtual bool IsDue()
    {
        return NextComputedExecution <= DateTime.UtcNow;
    }

    public void OnRequestCaptured(int _)
    {
        if (IsDue())
        {
            EventHandler.Set();
        }
    }

    public void Stop()
    {
        Stopped = true;
        EventHandler.Set();
    }

    public void WaitForNotification()
    {
        if (EventHandler.Reset())
        {
            EventHandler.WaitOne(GetWaitDelay());
        }
    }

    private TimeSpan GetWaitDelay()
    {
        var delay = NextComputedExecution - DateTime.UtcNow;

        return delay switch
        {
            var _ when delay >= TimeSpan.FromHours(1) => TimeSpan.FromHours(1),
            var _ when delay <= TimeSpan.Zero => TimeSpan.Zero,
            _ => delay
        };
    }
}
