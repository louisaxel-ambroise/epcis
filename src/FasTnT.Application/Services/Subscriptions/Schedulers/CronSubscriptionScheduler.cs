using FasTnT.Domain.Model.Subscriptions;

namespace FasTnT.Application.Services.Subscriptions.Schedulers;

public sealed class CronSubscriptionScheduler(SubscriptionSchedule schedule) : SubscriptionScheduler
{
    internal readonly ScheduleEntry
        Seconds = ScheduleEntry.Parse(schedule.Second, 0, 59),
        Minutes = ScheduleEntry.Parse(schedule.Minute, 0, 59),
        Hours = ScheduleEntry.Parse(schedule.Hour, 0, 23),
        DayOfMonth = ScheduleEntry.Parse(schedule.DayOfMonth, 1, 31),
        Month = ScheduleEntry.Parse(schedule.Month, 1, 12),
        DayOfWeek = ScheduleEntry.Parse(schedule.DayOfWeek, 1, 7);

    public override DateTime ComputeNextExecution(DateTime startDate)
    {
        var methods = new[] { SetSeconds, SetMinutes, SetHours, SetDayOfMonth, SetMonth };
        var tentative = methods.Aggregate(startDate.AddSeconds(1), (date, function) => function(date));

        if (!DayOfWeek.HasValue(1 + (int)tentative.DayOfWeek))
        {
            return ComputeNextExecution(new DateTime(tentative.Year, tentative.Month, tentative.Day, 23, 59, 59));
        }
        else
        {
            return tentative;
        }
    }

    private DateTime SetMinutes(DateTime tentative)
    {
        if (!Minutes.HasValue(tentative.Minute))
        {
            tentative = new DateTime(tentative.Year, tentative.Month, tentative.Day, tentative.Hour, Math.Max(tentative.Minute, Minutes.Min), Seconds.Min);
        }

        return GetNextTentative(tentative, x => x.Minute, x => x.AddMinutes(1), Minutes);
    }

    private DateTime SetHours(DateTime tentative)
    {
        if (!Hours.HasValue(tentative.Hour))
        {
            tentative = new DateTime(tentative.Year, tentative.Month, tentative.Day, Math.Max(tentative.Hour, Hours.Min), Minutes.Min, Seconds.Min);
        }

        return GetNextTentative(tentative, x => x.Hour, x => x.AddHours(1), Hours);
    }

    private DateTime SetDayOfMonth(DateTime tentative)
    {
        if (!DayOfMonth.HasValue(tentative.Day))
        {
            tentative = new DateTime(tentative.Year, tentative.Month, Math.Max(tentative.Day, DayOfMonth.Min), Hours.Min, Minutes.Min, Seconds.Min);
        }

        return GetNextTentative(tentative, x => x.Day, x => x.AddDays(1), DayOfMonth);
    }

    private DateTime SetMonth(DateTime tentative)
    {
        if (!Month.HasValue(tentative.Month))
        {
            tentative = new DateTime(tentative.Year, Math.Max(tentative.Month, Month.Min), DayOfMonth.Min, Hours.Min, Minutes.Min, Seconds.Min);
        }

        return GetNextTentative(tentative, x => x.Month, x => x.AddMonths(1), Month);
    }

    private DateTime SetSeconds(DateTime tentative)
    {
        return GetNextTentative(tentative, x => x.Second, x => x.AddSeconds(1), Seconds);
    }

    private static DateTime GetNextTentative(DateTime tentative, Func<DateTime, int> selector, Func<DateTime, DateTime> increment, ScheduleEntry entry)
    {
        while (!entry.HasValue(selector(tentative)))
        {
            tentative = increment(tentative);
        }

        return tentative;
    }

    internal sealed record ScheduleEntry
    {
        private readonly List<int> _values = [];
        private readonly int _minValue, _maxValue;

        public int Min => _values.Min();

        public static ScheduleEntry Parse(string expression, int min, int max) => new(expression, min, max);
        public bool HasValue(int value) => _values.Contains(value);

        private ScheduleEntry(string expression, int min, int max)
        {
            _minValue = min;
            _maxValue = max;

            ParseExpression(expression ?? string.Empty);
        }

        private void ParseExpression(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                AddRange(_minValue, _maxValue);
            }
            else
            {
                foreach (var element in expression.Split(','))
                {
                    ParseElement(element);
                }
            }
        }

        private void ParseElement(string element)
        {
            if (element.StartsWith('[') && element.EndsWith(']') && element.Contains('-'))
            {
                ParseRange(element);
            }
            else if (int.TryParse(element, out int value))
            {
                AddValue(value);
            }
            else
            {
                throw new ArgumentException($"Invalid value: {element}");
            }
        }

        private void ParseRange(string element)
        {
            var rangeParts = element[1..^1].Split('-');

            if (rangeParts.Length != 2)
            {
                throw new ArgumentException($"Invalid value: {element}");
            }
            if (int.TryParse(rangeParts[0], out int min) && int.TryParse(rangeParts[1], out int max))
            {
                AddRange(min, max);
            }
            else
            {
                throw new ArgumentException($"Invalid value: {element}");
            }
        }

        private void AddRange(int minValue, int maxValue)
        {
            if (minValue > maxValue || minValue < _minValue || maxValue > _maxValue)
            {
                throw new ArgumentException($"Invalid range value: [{minValue}-{maxValue}]");
            }

            _values.AddRange(Enumerable.Range(minValue, maxValue - minValue + 1));
        }

        private void AddValue(int value)
        {
            if (value < _minValue || value > _maxValue)
            {
                throw new ArgumentException($"Invalid value: {value}");
            }

            _values.Add(value);
        }
    }
}
