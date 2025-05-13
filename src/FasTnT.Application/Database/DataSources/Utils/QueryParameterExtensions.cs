using FasTnT.Application.Services;
using FasTnT.Application.Services.DataSources.Utils;
using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Queries;
using System.Globalization;
using System.Linq.Expressions;

namespace FasTnT.Application.Database.DataSources.Utils;

internal static class QueryParameterExtensions
{
    internal static int AsInt(this QueryParameter parameter) => int.Parse(parameter.AsString());
    internal static bool AsBool(this QueryParameter parameter) => bool.Parse(parameter.AsString());
    internal static double AsFloat(this QueryParameter parameter) => double.Parse(parameter.AsString(), CultureInfo.InvariantCulture);
    internal static DateTime AsDate(this QueryParameter parameter) => UtcDateTime.Parse(parameter.AsString());
    internal static bool IsDateTime(this QueryParameter parameter) => Regexs.Date().IsMatch(parameter.AsString());
    internal static bool IsNumeric(this QueryParameter parameter) => Regexs.Numeric().IsMatch(parameter.AsString());

    internal static string AsString(this QueryParameter parameter)
    {
        if (parameter.Values.Length != 1)
        {
            throw new EpcisException(ExceptionType.QueryParameterException, $"A single value is expected, but multiple were found. Parameter name '{parameter.Name}'");
        }

        return parameter.Values[0];
    }

    internal static string GetSimpleType(this QueryParameter parameter) => parameter.Name.Split('_', 3)[2];
    internal static string InnerIlmdName(this QueryParameter parameter) => parameter.Name.Split('_')[3].Split('#')[1];
    internal static string InnerIlmdNamespace(this QueryParameter parameter) => parameter.Name.Split('_')[3].Split('#')[0];
    internal static string IlmdName(this QueryParameter parameter) => parameter.Name.Split('_')[2].Split('#')[1];
    internal static string IlmdNamespace(this QueryParameter parameter) => parameter.Name.Split('_')[2].Split('#')[0];
    internal static string InnerFieldName(this QueryParameter parameter) => parameter.Name.Split('_')[2].Split('#')[1];
    internal static string InnerFieldNamespace(this QueryParameter parameter) => parameter.Name.Split('_')[2].Split('#')[0];
    internal static string SensorFieldName(this QueryParameter parameter) => parameter.Name.Split('_')[2].Split('#')[1];
    internal static string SensorFieldNamespace(this QueryParameter parameter) => parameter.Name.Split('_')[2].Split('#')[0];
    internal static FieldType SensorType(this QueryParameter parameter) => parameter.Name.Split('_')[1].Parse<FieldType>();
    internal static string InnerSensorFieldName(this QueryParameter parameter) => parameter.Name.Split('_')[3].Split('#')[1];
    internal static string InnerSensorFieldNamespace(this QueryParameter parameter) => parameter.Name.Split('_')[3].Split('#')[0];
    internal static FieldType InnerSensorType(this QueryParameter parameter) => parameter.Name.Split('_')[2].Parse<FieldType>();
    internal static string FieldName(this QueryParameter parameter) => parameter.Name.Split('_')[1].Split('#')[1];
    internal static string FieldNamespace(this QueryParameter parameter) => parameter.Name.Split('_')[1].Split('#')[0];
    internal static string AttributeName(this QueryParameter parameter) => parameter.Name.Split('_', 3)[2];
    internal static string ReportFieldUom(this QueryParameter parameter) => parameter.Name.Split('_', 3)[2];
    internal static string ReportField(this QueryParameter parameter) => Capitalize(parameter.Name.Split('_', 3)[1]);
    internal static string MasterdataType(this QueryParameter parameter) => parameter.Name.Split('_', 3)[1];

    internal static EpcType[] GetMatchEpcTypes(this QueryParameter parameter)
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

    internal static Expression<Func<T, bool>> Compare<T>(this QueryParameter parameter, Expression<Func<T, object>> accessor)
    {
        var param = Expression.Parameter(typeof(T));
        var right = parameter.IsDateTime() ? Expression.Constant(parameter.AsDate()) : Expression.Constant(parameter.AsFloat());
        var left = Expression.Convert(Expression.Invoke(accessor, param), right.Type);

        return parameter.Name[..2] switch
        {
            "GE" => Lambda<T>(Expression.GreaterThanOrEqual(left, right), param),
            "GT" => Lambda<T>(Expression.GreaterThan(left, right), param),
            "LE" => Lambda<T>(Expression.LessThanOrEqual(left, right), param),
            "LT" => Lambda<T>(Expression.LessThan(left, right), param),
            _ => throw new EpcisException(ExceptionType.QueryParameterException, $"Invalid comparison parameter: '{parameter.Name[..2]}'")
        };
    }

    internal static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        return Lambda<T>(Expression.AndAlso(expr1.Body, Expression.Invoke(expr2, expr1.Parameters)), expr1.Parameters);
    }

    internal static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        return Lambda<T>(Expression.OrElse(expr1.Body, Expression.Invoke(expr2, expr1.Parameters)), expr1.Parameters);
    }
    private static string Capitalize(string value) => char.ToUpper(value[0]) + value[1..];

    private static Expression<Func<T, bool>> Lambda<T>(BinaryExpression expr, params IEnumerable<ParameterExpression> parameters)
    {
        return Expression.Lambda<Func<T, bool>>(expr, parameters);
    }
}
