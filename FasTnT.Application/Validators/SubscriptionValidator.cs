using FasTnT.Domain.Model.Subscriptions;
using System.Text.RegularExpressions;

namespace FasTnT.Application.Validators;

public static class SubscriptionValidator
{
    public static bool IsValid(Subscription request)
    {
        if (!OnlyOneSubscriptionMethodIsDefined(request))
        {
            return false;
        }
        if (request.Schedule is not null && !IsValid(request.Schedule))
        {
            return false;
        }

        return true;
    }

    private static bool OnlyOneSubscriptionMethodIsDefined(Subscription request)
    {
        return string.IsNullOrEmpty(request.Trigger) != (request.Schedule is null);
    }

    private static bool IsValid(SubscriptionSchedule schedule)
    {
        return SecondRegex.IsMatch(schedule.Second)
            && MinuteRegex.IsMatch(schedule.Minute)
            && HourRegex.IsMatch(schedule.Hour)
            && DayOfMonthRegex.IsMatch(schedule.DayOfMonth)
            && MonthRegex.IsMatch(schedule.Month)
            && DayOfWeekRegex.IsMatch(schedule.DayOfWeek);
    }

    private readonly static Regex SecondRegex = BuildRegex("[0-5]?[0-9]");
    private readonly static Regex MinuteRegex = BuildRegex("[0-5]?[0-9]");
    private readonly static Regex HourRegex = BuildRegex("(2[0-3])|([0-1]?[0-9])");
    private readonly static Regex DayOfMonthRegex = BuildRegex("[1-9]|([1-2][0-9])|(3[0-1])");
    private readonly static Regex MonthRegex = BuildRegex("(1[0-2])|0?[1-9]");
    private readonly static Regex DayOfWeekRegex = BuildRegex("[1-7]");

    private static Regex BuildRegex(string range)
    {
        var element = $@"(({range})|(\[({range})\-({range})\]))";

        return new Regex($@"^({element}((\,{element})+)?)?$", RegexOptions.Compiled);
    }
}