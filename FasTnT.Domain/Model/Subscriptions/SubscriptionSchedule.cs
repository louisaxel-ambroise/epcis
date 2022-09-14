namespace FasTnT.Domain.Model.Subscriptions;

public class SubscriptionSchedule
{
    public Subscription Subscription { get; set; }
    public string Second { get; set; }
    public string Minute { get; set; }
    public string Hour { get; set; }
    public string DayOfMonth { get; set; }
    public string Month { get; set; }
    public string DayOfWeek { get; set; }

    public virtual DateTime GetNextOccurence(DateTime startDate)
    {
        // Parse from the next second
        var schedule = new SubscriptionScheduleEntry(this);
        var tentative = SetMonth(SetDayOfMonth(SetHours(SetMinutes(SetSeconds(startDate.AddSeconds(1), schedule), schedule), schedule), schedule), schedule);

        if (!schedule.DayOfWeek.HasValue(1 + (int)tentative.DayOfWeek))
        {
            return GetNextOccurence(new DateTime(tentative.Year, tentative.Month, tentative.Day, 23, 59, 59));
        }

        return tentative;
    }

    private static DateTime SetMinutes(DateTime tentative, SubscriptionScheduleEntry scheduleEntry)
    {
        if (!scheduleEntry.DayOfWeek.HasValue(tentative.Minute))
        {
            tentative = new DateTime(tentative.Year, tentative.Month, tentative.Day, tentative.Hour, Math.Max(tentative.Minute, scheduleEntry.Minutes.Min), scheduleEntry.Seconds.Min);
        }

        return GetNextTentative(tentative, x => x.Minute, x => x.AddMinutes(1), scheduleEntry.Minutes);
    }

    private static DateTime SetHours(DateTime tentative, SubscriptionScheduleEntry scheduleEntry)
    {
        if (!scheduleEntry.Hours.HasValue(tentative.Hour))
        {
            tentative = new DateTime(tentative.Year, tentative.Month, tentative.Day, Math.Max(tentative.Hour, scheduleEntry.Hours.Min), scheduleEntry.Minutes.Min, scheduleEntry.Seconds.Min);
        }

        return GetNextTentative(tentative, x => x.Hour, x => x.AddHours(1), scheduleEntry.Hours);
    }

    private static DateTime SetDayOfMonth(DateTime tentative, SubscriptionScheduleEntry scheduleEntry)
    {
        if (!scheduleEntry.DayOfMonth.HasValue(tentative.Day))
        {
            tentative = new DateTime(tentative.Year, tentative.Month, Math.Max(tentative.Day, scheduleEntry.DayOfMonth.Min), scheduleEntry.Hours.Min, scheduleEntry.Minutes.Min, scheduleEntry.Seconds.Min);
        }

        return GetNextTentative(tentative, x => x.Day, x => x.AddDays(1), scheduleEntry.DayOfMonth);
    }

    private static DateTime SetMonth(DateTime tentative, SubscriptionScheduleEntry scheduleEntry)
    {
        if (!scheduleEntry.Month.HasValue(tentative.Month))
        {
            tentative = new DateTime(tentative.Year, Math.Max(tentative.Month, scheduleEntry.Month.Min), scheduleEntry.DayOfMonth.Min, scheduleEntry.Hours.Min, scheduleEntry.Minutes.Min, scheduleEntry.Seconds.Min);
        }

        return GetNextTentative(tentative, x => x.Month, x => x.AddMonths(1), scheduleEntry.Month);
    }

    private static DateTime SetSeconds(DateTime tentative, SubscriptionScheduleEntry scheduleEntry)
    {
        return GetNextTentative(tentative, x => x.Second, x => x.AddSeconds(1), scheduleEntry.Seconds);
    }

    private static DateTime GetNextTentative(DateTime tentative, Func<DateTime, int> selector, Func<DateTime, DateTime> increment, ScheduleEntry entry)
    {
        while (!entry.HasValue(selector(tentative)))
        {
            tentative = increment(tentative);
        }

        return tentative;
    }
}
