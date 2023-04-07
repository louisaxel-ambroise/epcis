using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Host.Subscriptions.Schedulers;

public abstract class SubscriptionScheduler
{
    public bool Stopped { get; private set; }

    protected DateTime NextComputedExecution;
    protected ManualResetEvent EventHandler = new(false);

    public static SubscriptionScheduler Create(Subscription subscription) => Create(subscription.Trigger, subscription.Schedule);
    public static SubscriptionScheduler Create(string trigger, SubscriptionSchedule schedule)
    {
        if (!string.IsNullOrEmpty(trigger))
        {
            return TriggeredSubscriptionScheduler.Create(trigger);
        }
        else if (!IsEmpty(schedule))
        {
            return new CronSubscriptionScheduler(schedule);
        }
        else
        {
            throw new EpcisException(ExceptionType.SubscriptionControlsException, "Subscription trigger and/or schedule is invalid");
        }
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
