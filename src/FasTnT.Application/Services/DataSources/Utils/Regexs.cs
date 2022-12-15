using System.Text.RegularExpressions;

namespace FasTnT.Application.Services.DataSources.Utils;

public static partial class Regexs
{
    public static bool IsDate(string value) => Date().IsMatch(value);
    public static bool IsNumeric(string value) => Numeric().IsMatch(value);
    public static bool IsInnerIlmd(string value) => InnerIlmd().IsMatch(value);
    public static bool IsIlmd(string value) => Ilmd().IsMatch(value);
    public static bool IsInnerSensorElement(string value) => InnerSensorElement().IsMatch(value);
    public static bool IsSensorElement(string value) => SensorElement().IsMatch(value);
    public static bool IsInnerSensorMetadata(string value) => InnerSensorMetadata().IsMatch(value);
    public static bool IsSensorMetadata(string value) => SensorMetadata().IsMatch(value);
    public static bool IsInnerSensorReport(string value) => InnerSensorReport().IsMatch(value);
    public static bool IsSensorReport(string value) => SensorReport().IsMatch(value);
    public static bool IsInnerField(string value) => InnerField().IsMatch(value);
    public static bool IsField(string value) => Field().IsMatch(value);
    public static bool IsUoMField(string value) => UoMField().IsMatch(value);

    [GeneratedRegex("^-?\\d+(?:\\.\\d+)?$")]
    private static partial Regex Numeric();
    [GeneratedRegex("^([0-9]{4})-([0-9]{2})-([0-9]{2})")]
    private static partial Regex Date();
    [GeneratedRegex("^(GE|GT|LE|LT)_SENSORMETADATA_")]
    private static partial Regex SensorMetadata();
    [GeneratedRegex("^(GE|GT|LE|LT)_INNER_SENSORELEMENT_")]
    private static partial Regex InnerSensorElement();
    [GeneratedRegex("^(GE|GT|LE|LT)_INNER_ILMD_")]
    private static partial Regex InnerIlmd();
    [GeneratedRegex("^(GE|GT|LE|LT)_ILMD_")]
    private static partial Regex Ilmd();
    [GeneratedRegex("^(GE|GT|LE|LT)_SENSORELEMENT_")]
    private static partial Regex SensorElement();
    [GeneratedRegex("^(GE|GT|LE|LT)_INNER_SENSORMETADATA_")]
    private static partial Regex InnerSensorMetadata();
    [GeneratedRegex("^(GE|GT|LE|LT)_SENSOREPORT_")]
    private static partial Regex SensorReport();
    [GeneratedRegex("^(GE|GT|LE|LT)_INNER_SENSOREPORT_")]
    private static partial Regex InnerSensorReport();
    [GeneratedRegex("^(GE|GT|LE|LT)_INNER_")]
    private static partial Regex InnerField();
    [GeneratedRegex("^(GE|GT|LE|LT)_")]
    private static partial Regex Field();
    [GeneratedRegex("^(GE|GT|LE|LT)_[sDev|((min|max|mean|perc)Value)]_")]
    private static partial Regex UoMField();
}
