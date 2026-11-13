namespace FasTnT.Domain.Enumerations;

public static class Utils
{
    public static T Parse<T>(this string value) where T : struct
    {
        return Enum.Parse<T>(value, true);
    }
}