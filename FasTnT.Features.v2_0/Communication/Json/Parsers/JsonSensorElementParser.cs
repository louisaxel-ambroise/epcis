using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model;
using System.Text.Json;

namespace FasTnT.Features.v2_0.Communication.Json.Parsers;

internal static class JsonSensorElementParser
{

    public static SensorElement ParseSensorElement(JsonElement element)
    {
        var sensorElement = new SensorElement();

        foreach (var property in element.EnumerateObject())
        {
            switch (property.Name)
            {
                case "isA":
                    break; // Can it be something different from epcis:SensorElement?
                case "sensorMetadata":
                    ParseSensorMetadata(sensorElement, property.Value); break;
                case "sensorReport":
                    sensorElement.Reports.AddRange(ParseSensorReports(property.Value)); break;
                default:
                    throw new EpcisException(ExceptionType.ImplementationException, "Custom fields for SensorElement are not allowed yet.");
            }
        }

        return sensorElement;
    }

    private static IEnumerable<SensorReport> ParseSensorReports(JsonElement element)
    {
        return element.EnumerateArray().Select(ParseSensorReport);
    }

    private static SensorReport ParseSensorReport(JsonElement element)
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
                    report.Time = property.Value.GetDateTime(); break;
                case "microorganism":
                    report.Microorganism = property.Value.GetString(); break;
                case "chemicalSubstance":
                    report.ChemicalSubstance = property.Value.GetString(); break;
                case "value":
                    report.Value = property.Value.GetRawText(); break;
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
                    throw new EpcisException(ExceptionType.ImplementationException, "Custom fields for SensorElement are not allowed yet.");
            }
        }

        return report;
    }

    private static void ParseSensorMetadata(SensorElement sensorElement, JsonElement element)
    {
        foreach (var property in element.EnumerateObject())
        {
            switch (property.Name)
            {
                case "time":
                    sensorElement.Time = property.Value.GetDateTime(); break;
                case "deviceID":
                    sensorElement.DeviceId = property.Value.GetString(); break;
                case "deviceMetadata":
                    sensorElement.DeviceMetadata = property.Value.GetString(); break;
                case "rawData":
                    sensorElement.RawData = property.Value.GetString(); break;
                case "startTime":
                    sensorElement.StartTime = property.Value.GetDateTime(); break;
                case "endTime":
                    sensorElement.EndTime = property.Value.GetDateTime(); break;
                case "dataProcessingMethod":
                    sensorElement.DataProcessingMethod = property.Value.GetString(); break;
                case "bizRules":
                    sensorElement.BizRules = property.Value.GetString(); break;
                default:
                    throw new EpcisException(ExceptionType.ImplementationException, "Custom fields for SensorMetadata are not allowed yet.");
            }
        }
    }
}