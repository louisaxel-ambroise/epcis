using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Queries.Poll;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FasTnT.Domain.Utils
{
    public static class QueryParameterExtensions
    {
        public static int GetIntValue(this QueryParameter parameter) => int.Parse(parameter.Value());
        public static bool GetBoolValue(this QueryParameter parameter) => bool.Parse(parameter.Value());
        public static double GetNumeric(this QueryParameter parameter) => double.Parse(parameter.Value(), CultureInfo.InvariantCulture);
        public static DateTime GetDate(this QueryParameter parameter) => DateTime.Parse(parameter.Value(), null, DateTimeStyles.AdjustToUniversal);

        public static bool IsDateTime(this QueryParameter parameter) => Regex.IsMatch(parameter.Value(), "^([0-9]{4})-([0-9]{2})-([0-9]{2})");
        public static bool IsNumeric(this QueryParameter parameter) => Regex.IsMatch(parameter.Value(), @"^-?\d+(?:\.\d+)?$");

        public static string Value(this QueryParameter parameter)
        {
            if (parameter.Values.Length != 1)
            {
                throw new ArgumentException($"A single value is expected, but multiple were found. Parameter name '{parameter.Name}'");
            }

            return parameter.Values[0];
        }

        public static SourceDestinationType GetSourceDestinationType(this QueryParameter parameter)
        {
            return parameter.Name.StartsWith("EQ_source") ? SourceDestinationType.Source : SourceDestinationType.Destination;
        }

        public static string GetSimpleId(this QueryParameter parameter) => parameter.Name.Split('_', 3)[2];

        public static string InnerIlmdName(this QueryParameter parameter) => parameter.Name.Split('_')[3].Split('#')[1];
        public static string InnerIlmdNamespace(this QueryParameter parameter) => parameter.Name.Split('_')[3].Split('#')[0];
        public static string IlmdName(this QueryParameter parameter) => parameter.Name.Split('_')[2].Split('#')[1];
        public static string IlmdNamespace(this QueryParameter parameter) => parameter.Name.Split('_')[2].Split('#')[0];
        public static string InnerFieldName(this QueryParameter parameter) => parameter.Name.Split('_')[2].Split('#')[1];
        public static string InnerFieldNamespace(this QueryParameter parameter) => parameter.Name.Split('_')[2].Split('#')[0];
        public static string FieldName(this QueryParameter parameter) => parameter.Name.Split('_')[1].Split('#')[1];
        public static string FieldNamespace(this QueryParameter parameter) => parameter.Name.Split('_')[1].Split('#')[0];

        public static string GetAttributeName(this QueryParameter parameter) => parameter.Name.Split('_', 3)[2];

        public static EpcType[] GetMatchEpcTypes(this QueryParameter parameter)
        {
            if (!parameter.Name.StartsWith("MATCH_"))
            {
                throw new ArgumentException("A 'MATCH_*' parameter is expected here.");
            }

            return parameter.Name[6..] switch
            {
                "anyEPC" => new[] { EpcType.List, EpcType.ChildEpc, EpcType.ParentId, EpcType.InputEpc, EpcType.OutputEpc },
                "epc" => new[] { EpcType.List, EpcType.ChildEpc },
                "parentID" => new[] { EpcType.ParentId },
                "inputEPC" => new[] { EpcType.InputEpc },
                "outputEPC" => new[] { EpcType.OutputEpc },
                "epcClass" => new[] { EpcType.Quantity, EpcType.ChildQuantity },
                "inputEpcClass" => new[] { EpcType.InputQuantity },
                "outputEpcClass" => new[] { EpcType.OutputQuantity },
                "anyEpcClass" => new[] { EpcType.Quantity, EpcType.InputQuantity, EpcType.OutputQuantity },
                _ => throw new ArgumentException($"Unknown 'MATCH_*' parameter: '{parameter.Name}'")
            };
        }
    }
}
