namespace FasTnT.Domain.Model;

public class SubscriptionScheduleEntry
{
    internal readonly ScheduleEntry Seconds, 
                                    Minutes, 
                                    Hours, 
                                    DayOfMonth, 
                                    Month, 
                                    DayOfWeek;

    public SubscriptionScheduleEntry(SubscriptionSchedule schedule)
    {
        Seconds = ScheduleEntry.Parse(schedule.Second, 0, 60);
        Minutes = ScheduleEntry.Parse(schedule.Minute, 0, 59);
        Hours = ScheduleEntry.Parse(schedule.Hour, 0, 23);
        DayOfMonth = ScheduleEntry.Parse(schedule.DayOfMonth, 1, 31);
        Month = ScheduleEntry.Parse(schedule.Month, 1, 12);
        DayOfWeek = ScheduleEntry.Parse(schedule.DayOfWeek, 1, 7);
    }
}
