﻿using FasTnT.Domain.Enumerations;
using System.Text.RegularExpressions;

namespace FasTnT.Application.Services.DataSources.Utils;

public static partial class Regexs
{
    public static bool IsMatch(string value, Regex regex, out FieldType type)
    {
        var match = regex.Match(value);
        type = match.Success 
            ? Enum.Parse<FieldType>(match.Groups[nameof(type)].Value, true)
            : FieldType.Unknown;

        return match.Success;
    }

    [GeneratedRegex("^(GE|GT|LE|LT)_(?<type>SENSORMETADATA|SENSORELEMENT|SENSOREPORT)_")]
    public static partial Regex SensorFilter();
    [GeneratedRegex("^(GE|GT|LE|LT)_INNER_(?<type>SENSORMETADATA|SENSORELEMENT|SENSOREPORT)_")]
    public static partial Regex InnerSensorFilter();
    [GeneratedRegex("^-?\\d+(?:\\.\\d+)?$")]
    public static partial Regex Numeric();
    [GeneratedRegex("^([0-9]{4})-([0-9]{2})-([0-9]{2})")]
    public static partial Regex Date();
    [GeneratedRegex("^(GE|GT|LE|LT)_INNER_ILMD_")]
    public static partial Regex InnerIlmd();
    [GeneratedRegex("^(GE|GT|LE|LT)_ILMD_")]
    public static partial Regex Ilmd();
    [GeneratedRegex("^(GE|GT|LE|LT)_INNER_")]
    public static partial Regex InnerField();
    [GeneratedRegex("^(GE|GT|LE|LT)_")]
    public static partial Regex Field();
    [GeneratedRegex("^(GE|GT|LE|LT)_(sDev|((min|max|mean|perc)Value))_")]
    public static partial Regex UoMField();
}
