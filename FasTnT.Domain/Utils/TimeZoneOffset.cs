using System;

namespace FasTnT.Domain.Utils
{
    public class TimeZoneOffset
    {
        public static TimeZoneOffset Default => new TimeZoneOffset();

        public string Representation { get { return ComputeRepresentation(Value); } set { Value = ComputeValue(value); } }
        public short Value { get; set; }

        private static string ComputeRepresentation(int value)
        {
            var sign = value >= 0 ? "+" : "-";
            var hours = (Math.Abs(value) / 60).ToString("D2");
            var minutes = (Math.Abs(value % 60)).ToString("D2");

            return string.Format("{0}{1}:{2}", sign, hours, minutes);
        }

        private static short ComputeValue(string value)
        {
            var sign = (value[0] == '-') ? -1 : +1;
            var parts = value.Split(':');

            return (short)(sign * (Math.Abs(int.Parse(parts[0])) * 60 + int.Parse(parts[1])));
        }
    }
}
