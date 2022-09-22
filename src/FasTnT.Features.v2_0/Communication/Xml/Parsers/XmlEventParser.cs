using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Events;

namespace FasTnT.Features.v2_0.Communication.Xml.Parsers;

public static class XmlEventParser
{
    public static IEnumerable<Event> ParseEvents(XElement root)
    {
        return root.Elements().Select(ParseEvent);
    }

    private static Event ParseEvent(XElement element)
    {
        var evt = new Event
        {
            Type = Enum.Parse<EventType>(element.Name.LocalName)
        };

        foreach (var field in element.Elements())
        {
            if (string.IsNullOrEmpty(field.Name.NamespaceName))
            {
                switch (field.Name.LocalName)
                {
                    case "action":
                        evt.Action = Enum.Parse<EventAction>(field.Value, true); break;
                    case "eventTime":
                        evt.EventTime = DateTime.Parse(field.Value); break;
                    case "eventTimeZoneOffset":
                        evt.EventTimeZoneOffset = field.Value; break;
                    case "bizStep":
                        evt.BusinessStep = field.Value; break;
                    case "disposition":
                        evt.Disposition = field.Value; break;
                    case "transformationID":
                        evt.TransformationId = field.Value; break;
                    case "readPoint":
                        evt.ReadPoint = field.Element("id").Value; break;
                    case "bizLocation":
                        evt.BusinessLocation = field.Element("id").Value; break;
                    case "eventID":
                        evt.EventId = field.Value; break;
                    case "parentID":
                        evt.Epcs.Add(new Epc { Type = EpcType.ParentId, Id = field.Value }); break;
                    case "epcList":
                        evt.Epcs.AddRange(ParseEpcList(field, EpcType.List)); break;
                    case "childEPCs":
                        evt.Epcs.AddRange(ParseEpcList(field, EpcType.ChildEpc)); break;
                    case "inputEPCList":
                        evt.Epcs.AddRange(ParseEpcList(field, EpcType.InputEpc)); break;
                    case "outputEPCList":
                        evt.Epcs.AddRange(ParseEpcList(field, EpcType.OutputEpc)); break;
                    case "quantityList":
                        evt.Epcs.AddRange(ParseQuantityEpcList(field, EpcType.Quantity)); break;
                    case "childQuantityList":
                        evt.Epcs.AddRange(ParseQuantityEpcList(field, EpcType.ChildQuantity)); break;
                    case "inputQuantityList":
                        evt.Epcs.AddRange(ParseQuantityEpcList(field, EpcType.InputQuantity)); break;
                    case "outputQuantityList":
                        evt.Epcs.AddRange(ParseQuantityEpcList(field, EpcType.InputQuantity)); break;
                    case "bizTransactionList":
                        evt.Transactions.AddRange(ParseTransactionList(field)); break;
                    case "sourceList":
                        evt.Sources.AddRange(ParseSourceList(field)); break;
                    case "destinationList":
                        evt.Destinations.AddRange(ParseDestinationList(field)); break;
                    case "persistentDisposition":
                        evt.PersistentDispositions.AddRange(ParsePersustentDisposition(field)); break;
                    case "sensorElementList":
                        evt.SensorElements.AddRange(XmlSensorParser.ParseSensorElements(field)); break;
                    case "ilmd":
                        ParseIlmd(evt, field); break;
                    default:
                        throw new EpcisException(ExceptionType.ImplementationException, $"Unexpected event field: {field.Name}");
                }
            }
            else
            {
                evt.Fields.Add(XmlCustomFieldParser.ParseCustomFields(field, FieldType.CustomField));
            }
        }

        return evt;
    }

    private static void ParseIlmd(Event evt, XElement element)
    {
        foreach (var field in element.Elements())
        {
            evt.Fields.Add(XmlCustomFieldParser.ParseCustomFields(field, FieldType.Ilmd));
        }
    }

    private static IEnumerable<Epc> ParseEpcList(XElement field, EpcType type)
    {
        return field.Elements().Select(x => new Epc
        {
            Id = x.Value,
            Type = type
        });
    }

    private static IEnumerable<Epc> ParseQuantityEpcList(XElement field, EpcType type)
    {
        return field.Elements().Select(x => new Epc
        {
            Id = x.Element("epcClass").Value,
            Quantity = float.Parse(x.Element("quantity")?.Value),
            UnitOfMeasure = x.Element("uom")?.Value,
            IsQuantity = true,
            Type = type,
        });
    }

    private static IEnumerable<BusinessTransaction> ParseTransactionList(XElement field)
    {
        return field.Elements().Select(x => new BusinessTransaction
        {
            Id = x.Value,
            Type = x.Attribute("type").Value
        });
    }

    private static IEnumerable<Source> ParseSourceList(XElement field)
    {
        return field.Elements().Select(x => new Source
        {
            Id = x.Value,
            Type = x.Attribute("type").Value
        });
    }

    private static IEnumerable<Destination> ParseDestinationList(XElement field)
    {
        return field.Elements().Select(x => new Destination
        {
            Id = x.Value,
            Type = x.Attribute("type").Value
        });
    }

    private static IEnumerable<PersistentDisposition> ParsePersustentDisposition(XElement field)
    {
        return field.Elements().Select(x => new PersistentDisposition
        {
            Id = x.Value,
            Type = Enum.Parse<PersistentDispositionType>(x.Name.LocalName, true)
        });
    }
}

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
                switch (field.Name.LocalName)
                {
                    case "sensorMetadata":
                        ParseSensorMetadata(sensorElement, field); break;
                    case "sensorReport":
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
