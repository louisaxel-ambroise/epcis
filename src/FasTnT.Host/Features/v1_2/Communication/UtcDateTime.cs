namespace FasTnT.Host.Features.v1_2.Communication;

public static class UtcDateTime
{
    public static readonly DateTimeStyles Styles = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal;

    public static DateTime Parse(string value)
    {
        return DateTime.Parse(value, default, Styles);
    }

    public static bool TryParse(string value, out DateTime result)
    {
        return DateTime.TryParse(value, null, Styles, out result);
    }
}
