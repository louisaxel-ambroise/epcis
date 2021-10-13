using System;
using System.Collections.Generic;
using System.Linq;

namespace FasTnT.Domain.Model
{
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
            var scheduleEntry = new SubscriptionScheduleEntry(this);
            var tentative = SetMonth(SetDayOfMonth(SetHours(SetMinutes(SetSeconds(startDate.AddSeconds(1), scheduleEntry), scheduleEntry), scheduleEntry), scheduleEntry), scheduleEntry); // Parse from the next second

            if (!scheduleEntry.DayOfWeek.HasValue(1 + (int)tentative.DayOfWeek))
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

    public class ScheduleEntry
    {
        private readonly List<int> _values = new ();
        private readonly int _minValue, _maxValue;

        public int Min => _values.Min();

        public static ScheduleEntry Parse(string expression, int min, int max) => new (expression, min, max);
        public bool HasValue(int value) => _values.Contains(value);

        private ScheduleEntry(string expression, int min, int max)
        {
            _minValue = min;
            _maxValue = max;

            ParseExpression(expression);
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
            if (element.StartsWith("[") && element.EndsWith(']') && element.Contains('-'))
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

            _values.AddRange(Enumerable.Range(minValue, (maxValue - minValue + 1)));
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
