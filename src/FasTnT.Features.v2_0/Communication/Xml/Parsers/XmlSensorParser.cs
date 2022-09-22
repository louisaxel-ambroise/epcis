using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Events;

namespace FasTnT.Features.v2_0.Communication.Xml.Parsers;

public static class XmlSensorParser
{
    public static IEnumerable<SensorElement> ParseSensorElements(XElement field)
    {
        return field.Elements().Select(ParseSensorElement);
    }

    private static SensorElement ParseSensorElement(XElement element)
    {
        var sensorElement = new SensorElement();

        foreach (var field in element.Elements())
        {
            if (string.IsNullOrEmpty(field.Name.NamespaceName))
            {
                if (field.Name.LocalName == "sensorMetadata")
                {
                    ParseSensorMetadata(sensorElement, field);
                }
                else if (field.Name.LocalName == "sensorReport")
                {
                    ParseSensorReport(sensorElement, field); break;
                }
            }
            else
            {
                sensorElement.Fields.Add(XmlCustomFieldParser.ParseCustomFields(field, FieldType.Sensor));
            }
        }

        return sensorElement;
    }

    private static void ParseSensorReport(SensorElement sensorElement, XElement element)
    {
        var report = new SensorReport();

        foreach (var field in element.Attributes())
        {
            if (string.IsNullOrEmpty(field.Name.NamespaceName))
            {
                switch (field.Name.LocalName)
                {
                    case "value":
                        report.Value = float.Parse(field.Value); break;
                    case "type":
                        report.Type = field.Value; break;
                    case "component":
                        report.Component = field.Value; break;
                    case "stringValue":
                        report.StringValue = field.Value; break;
                    case "booleanValue":
                        report.BooleanValue = bool.Parse(field.Value); break;
                    case "hexBinaryValue":
                        report.HexBinaryValue = field.Value; break;
                    case "uriValue":
                        report.UriValue = field.Value; break;
                    case "uom":
                        report.UnitOfMeasure = field.Value; break;
                    case "minValue":
                        report.MinValue = float.Parse(field.Value); break;
                    case "maxValue":
                        report.MaxValue = float.Parse(field.Value); break;
                    case "sDev":
                        report.SDev = float.Parse(field.Value); break;
                    case "chemicalSubstance":
                        report.ChemicalSubstance = field.Value; break;
                    case "microorganism":
                        report.Microorganism = field.Value; break;
                    case "deviceID":
                        report.DeviceId = field.Value; break;
                    case "deviceMetadata":
                        report.DeviceMetadata = field.Value; break;
                    case "rawData":
                        report.RawData = field.Value; break;
                    case "time":
                        report.Time = DateTime.Parse(field.Value); break;
                    case "meanValue":
                        report.MeanValue = float.Parse(field.Value); break;
                    case "percRank":
                        report.PercRank = float.Parse(field.Value); break;
                    case "percValue":
                        report.PercValue = float.Parse(field.Value); break;
                    case "dataProcessingMethod":
                        report.DataProcessingMethod = field.Value; break;
                    default:
                        throw new EpcisException(ExceptionType.ImplementationException, $"Unexpected event field: {field.Name}");
                }
            }
            else
            {
                report.Fields.Add(XmlCustomFieldParser.ParseCustomFields(field, FieldType.SensorReport));
            }
        }

        sensorElement.Reports.Add(report);
    }

    private static void ParseSensorMetadata(SensorElement sensorElement, XElement metadata)
    {
        foreach (var field in metadata.Attributes())
        {
            if (string.IsNullOrEmpty(field.Name.NamespaceName))
            {
                switch (field.Name.LocalName)
                {
                    case "time":
                        sensorElement.Time = DateTime.Parse(field.Value); break;
                    case "bizRules":
                        sensorElement.BizRules = field.Value; break;
                    case "deviceID":
                        sensorElement.DeviceId = field.Value; break;
                    case "deviceMetadata":
                        sensorElement.DeviceMetadata = field.Value; break;
                    case "rawData":
                        sensorElement.RawData = field.Value; break;
                    case "startTime":
                        sensorElement.StartTime = DateTime.Parse(field.Value); break;
                    case "endTime":
                        sensorElement.EndTime = DateTime.Parse(field.Value); break;
                    case "dataProcessingMethod":
                        sensorElement.DataProcessingMethod = field.Value; break;
                    default:
                        throw new EpcisException(ExceptionType.ImplementationException, $"Unexpected event field: {field.Name}");
                }
            }
            else
            {
                sensorElement.Fields.Add(XmlCustomFieldParser.ParseCustomFields(field, FieldType.SensorMetadata));
            }
        }
    }
}
