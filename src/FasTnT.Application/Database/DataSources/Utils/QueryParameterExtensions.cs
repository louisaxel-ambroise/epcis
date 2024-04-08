using FasTnT.Application.Services.DataSources.Utils;
using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Queries;
using System.Globalization;
using System.Linq.Expressions;

namespace FasTnT.Application.Database.DataSources.Utils;

internal static class QueryParameterExtensions
{
    public static int AsInt(this QueryParameter parameter) => int.Parse(parameter.AsString());
    public static bool AsBool(this QueryParameter parameter) => bool.Parse(parameter.AsString());
    public static double AsFloat(this QueryParameter parameter) => double.Parse(parameter.AsString(), CultureInfo.InvariantCulture);
    public static DateTime AsDate(this QueryParameter parameter) => DateTime.Parse(parameter.AsString(), null, DateTimeStyles.AdjustToUniversal);
    public static bool IsDateTime(this QueryParameter parameter) => Regexs.Date().IsMatch(parameter.AsString());
    public static bool IsNumeric(this QueryParameter parameter) => Regexs.Numeric().IsMatch(parameter.AsString());

    public static string AsString(this QueryParameter parameter)
    {
        if (parameter.Values.Length != 1)
        {
            throw new EpcisException(ExceptionType.QueryParameterException, $"A single value is expected, but multiple were found. Parameter name '{parameter.Name}'");
        }

        return parameter.Values[0];
    }

    public static string GetSimpleId(this QueryParameter parameter) => parameter.Name.Split('_', 3)[2];
    public static string InnerIlmdName(this QueryParameter parameter) => parameter.Name.Split('_')[3].Split('#')[1];
    public static string InnerIlmdNamespace(this QueryParameter parameter) => parameter.Name.Split('_')[3].Split('#')[0];
    public static string IlmdName(this QueryParameter parameter) => parameter.Name.Split('_')[2].Split('#')[1];
    public static string IlmdNamespace(this QueryParameter parameter) => parameter.Name.Split('_')[2].Split('#')[0];
    public static string InnerFieldName(this QueryParameter parameter) => parameter.Name.Split('_')[2].Split('#')[1];
    public static string InnerFieldNamespace(this QueryParameter parameter) => parameter.Name.Split('_')[2].Split('#')[0];
    public static string SensorFieldName(this QueryParameter parameter) => parameter.Name.Split('_')[2].Split('#')[1];
    public static string SensorFieldNamespace(this QueryParameter parameter) => parameter.Name.Split('_')[2].Split('#')[0];
    public static FieldType SensorType(this QueryParameter parameter) => Enum.Parse<FieldType>(parameter.Name.Split('_')[1], true);
    public static string InnerSensorFieldName(this QueryParameter parameter) => parameter.Name.Split('_')[3].Split('#')[1];
    public static string InnerSensorFieldNamespace(this QueryParameter parameter) => parameter.Name.Split('_')[3].Split('#')[0];
    public static FieldType InnerSensorType(this QueryParameter parameter) => Enum.Parse<FieldType>(parameter.Name.Split('_')[2], true);
    public static string FieldName(this QueryParameter parameter) => parameter.Name.Split('_')[1].Split('#')[1];
    public static string FieldNamespace(this QueryParameter parameter) => parameter.Name.Split('_')[1].Split('#')[0];
    public static string AttributeName(this QueryParameter parameter) => parameter.Name.Split('_', 3)[2];
    public static string ReportFieldUom(this QueryParameter parameter) => parameter.Name.Split('_', 3)[2];
    public static string ReportField(this QueryParameter parameter) => Capitalize(parameter.Name.Split('_', 3)[1]);
    public static string MasterdataType(this QueryParameter parameter) => parameter.Name.Split('_', 3)[1];

    private static string Capitalize(string value) => char.ToUpper(value[0]) + value[1..];

    public static EpcType[] GetMatchEpcTypes(this QueryParameter parameter)
    {
        if (!parameter.Name.StartsWith("MATCH_"))
        {
            throw new EpcisException(ExceptionType.QueryParameterException, "A 'MATCH_*' parameter is expected here.");
        }

        return parameter.Name[6..] switch
        {
            "anyEPC" => [EpcType.List, EpcType.ChildEpc, EpcType.ParentId, EpcType.InputEpc, EpcType.OutputEpc],
            "epc" => [EpcType.List, EpcType.ChildEpc],
            "parentID" => [EpcType.ParentId],
            "inputEPC" => [EpcType.InputEpc],
            "outputEPC" => [EpcType.OutputEpc],
            "epcClass" => [EpcType.Quantity, EpcType.ChildQuantity],
            "inputEpcClass" => [EpcType.InputQuantity],
            "outputEpcClass" => [EpcType.OutputQuantity],
            "anyEpcClass" => [EpcType.Quantity, EpcType.InputQuantity, EpcType.OutputQuantity],
            _ => throw new EpcisException(ExceptionType.QueryParameterException, $"Unknown 'MATCH_*' parameter: '{parameter.Name}'")
        };
    }

    public static Expression<Func<T, bool>> Compare<T>(this QueryParameter parameter, Expression<Func<T, object>> accessor)
    {
        var param = Expression.Parameter(typeof(T));
        var right = parameter.IsDateTime() ? Expression.Constant(parameter.AsDate()) : Expression.Constant(parameter.AsFloat());
        var left = Expression.Convert(Expression.Invoke(accessor, param), right.Type);

        return parameter.Name[..2] switch
        {
            "GE" => Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(left, right), param),
            "GT" => Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(left, right), param),
            "LE" => Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(left, right), param),
            "LT" => Expression.Lambda<Func<T, bool>>(Expression.LessThan(left, right), param),
            _ => throw new EpcisException(ExceptionType.QueryParameterException, $"Invalid comparison parameter: '{parameter.Name[..2]}'")
        };
    }

    public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, Expression.Invoke(expr2, expr1.Parameters[0])), expr1.Parameters[0]);
    }

    public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, Expression.Invoke(expr2, expr1.Parameters[0])), expr1.Parameters[0]);
    }
}
