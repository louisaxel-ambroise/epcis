namespace FasTnT.Domain.Infrastructure.Utils;

public class TimeZoneOffset
{
    public static TimeZoneOffset Default => new();

    public string Representation { get { return ComputeRepresentation(Value); } set { Value = ComputeValue(value); } }
    public short Value { get; set; }

    private static string ComputeRepresentation(int value)
    {
        var sign = value >= 0 ? "+" : "-";
        var hours = (Math.Abs(value) / 60).ToString("D2");
        var minutes = (Math.Abs(value) % 60).ToString("D2");

        return string.Format("{0}{1}:{2}", sign, hours, minutes);
    }

    private static short ComputeValue(string value)
    {
        try
        {
            var sign = value[0] is '-' ? -1 : +1;
            var parts = value.TrimStart('+', '-').Split(':');

            return (short)(sign * (int.Parse(parts[0]) * 60 + int.Parse(parts[1])));
        }
        catch
        {
            throw new FormatException($"Invalid format for TimeZoneOffset: {value}");
        }
    }

    public static implicit operator TimeZoneOffset(string representation) => new() { Representation = representation };
    public static implicit operator TimeZoneOffset(short value) => new() { Value = value };
}
