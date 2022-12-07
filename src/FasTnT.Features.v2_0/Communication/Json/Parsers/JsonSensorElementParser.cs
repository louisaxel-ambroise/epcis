using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model.Events;
using System.Text.Json;

namespace FasTnT.Features.v2_0.Communication.Json.Parsers;

internal static class JsonSensorElementParser
{
    public static SensorElement ParseSensorElement(JsonElement element, Namespaces namespaces)
    {
        var sensorElement = new SensorElement();

        foreach (var property in element.EnumerateObject())
        {
            switch (property.Name)
            {
                case "type":
                    break; // Can it be something different from epcis:SensorElement?
                case "sensorMetadata":
                    ParseSensorMetadata(sensorElement, property.Value, namespaces); break;
                case "sensorReport":
                    sensorElement.Reports.AddRange(ParseSensorReports(property.Value, namespaces)); break;
                default:
                    sensorElement.Fields.AddRange(ParseCustomField(property, FieldType.Sensor, namespaces)); break;
            }
        }

        return sensorElement;
    }

    private static IEnumerable<SensorReport> ParseSensorReports(JsonElement element, Namespaces namespaces)
    {
        return element.EnumerateArray().Select(x => ParseSensorReport(x, namespaces));
    }

    private static SensorReport ParseSensorReport(JsonElement element, Namespaces namespaces)
    {
        var report = new SensorReport();

        foreach (var property in element.EnumerateObject())
        {
            switch (property.Name)
            {
                case "type":
                    report.Type = property.Value.GetString(); break;
                case "deviceID":
                    report.DeviceId = property.Value.GetString(); break;
                case "rawData":
                    report.RawData = property.Value.GetString(); break;
                case "dataProcessingMethod":
                    report.DataProcessingMethod = property.Value.GetString(); break;
                case "time":
                    report.Time = property.Value.GetDateTimeOffset(); break;
                case "microorganism":
                    report.Microorganism = property.Value.GetString(); break;
                case "chemicalSubstance":
                    report.ChemicalSubstance = property.Value.GetString(); break;
                case "value":
                    report.Value = property.Value.GetSingle(); break;
                case "component":
                    report.Component = property.Value.GetString(); break;
                case "stringValue":
                    report.StringValue = property.Value.GetString(); break;
                case "booleanValue":
                    report.BooleanValue = property.Value.GetBoolean(); break;
                case "hexBinaryValue":
                    report.HexBinaryValue = property.Value.GetString(); break;
                case "uriValue":
                    report.UriValue = property.Value.GetString(); break;
                case "minValue":
                    report.MaxValue = property.Value.GetSingle(); break;
                case "maxValue":
                    report.MinValue = property.Value.GetSingle(); break;
                case "meanValue":
                    report.MeanValue = property.Value.GetSingle(); break;
                case "percRank":
                    report.PercRank = property.Value.GetSingle(); break;
                case "percValue":
                    report.PercValue = property.Value.GetSingle(); break;
                case "uom":
                    report.UnitOfMeasure = property.Value.GetString(); break;
                case "sDev":
                    report.SDev = property.Value.GetSingle(); break;
                case "deviceMetadata":
                    report.DeviceMetadata = property.Value.GetString(); break;
                default:
                    report.Fields.AddRange(ParseCustomField(property, FieldType.SensorReport, namespaces)); break;
            }
        }

        return report;
    }

    private static void ParseSensorMetadata(SensorElement sensorElement, JsonElement element, Namespaces namespaces)
    {
        foreach (var property in element.EnumerateObject())
        {
            switch (property.Name)
            {
                case "time":
                    sensorElement.Time = property.Value.GetDateTimeOffset(); break;
                case "deviceID":
                    sensorElement.DeviceId = property.Value.GetString(); break;
                case "deviceMetadata":
                    sensorElement.DeviceMetadata = property.Value.GetString(); break;
                case "rawData":
                    sensorElement.RawData = property.Value.GetString(); break;
                case "startTime":
                    sensorElement.StartTime = property.Value.GetDateTimeOffset(); break;
                case "endTime":
                    sensorElement.EndTime = property.Value.GetDateTimeOffset(); break;
                case "dataProcessingMethod":
                    sensorElement.DataProcessingMethod = property.Value.GetString(); break;
                case "bizRules":
                    sensorElement.BizRules = property.Value.GetString(); break;
                default:
                    sensorElement.Fields.AddRange(ParseCustomField(property, FieldType.SensorMetadata, namespaces)); break;
            }
        }
    }

    private static IEnumerable<Field> ParseCustomField(JsonProperty jsonProperty, FieldType type, Namespaces namespaces)
    {
        var (ns, name) = ParseName(jsonProperty.Name, namespaces);
        return ParseCustomField(jsonProperty.Value, type, name, ns, namespaces);
    }

    private static IEnumerable<Field> ParseCustomField(JsonElement element, FieldType type, string propName, string propNs, Namespaces namespaces)
    {
        var field = new Field { Type = type, Name = propName, Namespace = propNs };

        if (element.ValueKind == JsonValueKind.Object)
        {
            field.Children.AddRange(element.EnumerateObject().SelectMany(e =>
            {
                var (ns, name) = ParseName(e.Name, namespaces);
                return ParseCustomField(e.Value, type, name, ns, namespaces);
            }));
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            return element.EnumerateArray().SelectMany(e => ParseCustomField(e, type, propName, propNs, namespaces));
        }
        else
        {
            field.TextValue = element.GetString();
            field.NumericValue = float.TryParse(field.TextValue, out float numericValue) ? numericValue : default(float?);
            field.DateValue = DateTimeOffset.TryParse(field.TextValue, null, DateTimeStyles.AdjustToUniversal, out DateTimeOffset dateValue) ? dateValue : default(DateTimeOffset?);
        }

        return new[] { field };
    }

    private static (string Namespace, string Name) ParseName(string name, Namespaces namespaces)
    {
        var parts = name.Split(':', 2);

        return (namespaces[parts[0]], parts[1]);
    }
}