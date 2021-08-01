using FluentValidation;
using System.Text.RegularExpressions;

namespace FasTnT.Domain.Commands.Subscribe
{
    public class SubscribeCommandValidator : AbstractValidator<SubscribeCommand>
    {
        // TODO: validate URL. Uri.IsWellFormedUriString does not support basic auth...
        public SubscribeCommandValidator()
        {
            RuleFor(x => x.Trigger).Null().When(x => x.Schedule is not null);
            RuleFor(x => x.Schedule).Null().When(x => x.Trigger is not null);

            RuleFor(x => x.Schedule).Must(BeAValidSchedule).When(x => x.Schedule is not null);
        }

        private bool BeAValidSchedule(QuerySchedule schedule) => QueryScheduleValidator.IsValid(schedule);

        internal static class QueryScheduleValidator
        {
            public static bool IsValid(QuerySchedule schedule)
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
    }
}
