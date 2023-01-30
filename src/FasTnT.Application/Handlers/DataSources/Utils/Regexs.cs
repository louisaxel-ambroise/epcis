using FasTnT.Domain.Enumerations;
using System.Text.RegularExpressions;

namespace FasTnT.Application.Services.DataSources.Utils;

public static partial class Regexs
{
    public static bool IsDate(string value) => Date().IsMatch(value);
    public static bool IsNumeric(string value) => Numeric().IsMatch(value);
    public static bool IsInnerIlmd(string value) => InnerIlmd().IsMatch(value);
    public static bool IsIlmd(string value) => Ilmd().IsMatch(value);
    public static bool IsInnerField(string value) => InnerField().IsMatch(value);
    public static bool IsField(string value) => Field().IsMatch(value);
    public static bool IsUoMField(string value) => UoMField().IsMatch(value);

    public static bool IsSensorFilter(string value, out FieldType type)
    {
        var match = SensorFilter().Match(value);
        type = match.Success 
            ? Enum.Parse<FieldType>(match.Groups[2].Value, true)
            : FieldType.Unknown;

        return match.Success;
    }
    public static bool IsInnerSensorFilter(string value, out FieldType type)
    {
        var match = InnerSensorFilter().Match(value);
        type = match.Success 
            ? Enum.Parse<FieldType>(match.Groups[2].Value, true)
            : FieldType.Unknown;

        return match.Success;
    }

    [GeneratedRegex("^(GE|GT|LE|LT)_(SENSORMETADATA|SENSORELEMENT|SENSOREPORT)_")]
    private static partial Regex SensorFilter();
    [GeneratedRegex("^(GE|GT|LE|LT)_INNER_(SENSORMETADATA|SENSORELEMENT|SENSOREPORT)_")]
    private static partial Regex InnerSensorFilter();

    [GeneratedRegex("^-?\\d+(?:\\.\\d+)?$")]
    private static partial Regex Numeric();
    [GeneratedRegex("^([0-9]{4})-([0-9]{2})-([0-9]{2})")]
    private static partial Regex Date();
    [GeneratedRegex("^(GE|GT|LE|LT)_INNER_ILMD_")]
    private static partial Regex InnerIlmd();
    [GeneratedRegex("^(GE|GT|LE|LT)_ILMD_")]
    private static partial Regex Ilmd();
    [GeneratedRegex("^(GE|GT|LE|LT)_INNER_")]
    private static partial Regex InnerField();
    [GeneratedRegex("^(GE|GT|LE|LT)_")]
    private static partial Regex Field();
    [GeneratedRegex("^(GE|GT|LE|LT)_(sDev|((min|max|mean|perc)Value))_")]
    private static partial Regex UoMField();
}
