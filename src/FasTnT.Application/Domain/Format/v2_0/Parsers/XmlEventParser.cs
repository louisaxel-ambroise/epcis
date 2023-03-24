using FasTnT.Application.Domain.Exceptions;
using FasTnT.Application.Domain.Model.Events;

namespace FasTnT.Application.Domain.Format.v2_0.Parsers;

public class XmlEventParser
{
    private int _index;
    private Event _evt;

    public static IEnumerable<Event> ParseEvents(XElement root)
    {
        return root.Elements().Select(ParseEvent);
    }

    public static Event ParseEvent(XElement element)
    {
        var parser = new XmlEventParser();

        return parser.Parse(element);
    }

    internal Event Parse(XElement element)
    {
        _evt = new Event
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
                        _evt.Action = Enum.Parse<EventAction>(field.Value, true); break;
                    case "recordTime": // Discard - this will be overridden
                        break;
                    case "eventTime":
                        _evt.EventTime = DateTime.Parse(field.Value, null, DateTimeStyles.AdjustToUniversal); break;
                    case "certificationInfo":
                        _evt.CertificationInfo = field.Value; break;
                    case "eventTimeZoneOffset":
                        _evt.EventTimeZoneOffset = field.Value; break;
                    case "bizStep":
                        _evt.BusinessStep = field.Value; break;
                    case "disposition":
                        _evt.Disposition = field.Value; break;
                    case "transformationID":
                        _evt.TransformationId = field.Value; break;
                    case "readPoint":
                        ParseReadPoint(field); break;
                    case "bizLocation":
                        ParseBizLocation(field); break;
                    case "eventID":
                        _evt.EventId = field.Value; break;
                    case "parentID":
                        _evt.Epcs.Add(new Epc { Type = EpcType.ParentId, Id = field.Value }); break;
                    case "epcList":
                        _evt.Epcs.AddRange(ParseEpcList(field, EpcType.List)); break;
                    case "childEPCs":
                        _evt.Epcs.AddRange(ParseEpcList(field, EpcType.ChildEpc)); break;
                    case "inputEPCList":
                        _evt.Epcs.AddRange(ParseEpcList(field, EpcType.InputEpc)); break;
                    case "outputEPCList":
                        _evt.Epcs.AddRange(ParseEpcList(field, EpcType.OutputEpc)); break;
                    case "quantityList":
                        _evt.Epcs.AddRange(ParseQuantityEpcList(field, EpcType.Quantity)); break;
                    case "childQuantityList":
                        _evt.Epcs.AddRange(ParseQuantityEpcList(field, EpcType.ChildQuantity)); break;
                    case "inputQuantityList":
                        _evt.Epcs.AddRange(ParseQuantityEpcList(field, EpcType.InputQuantity)); break;
                    case "outputQuantityList":
                        _evt.Epcs.AddRange(ParseQuantityEpcList(field, EpcType.OutputQuantity)); break;
                    case "bizTransactionList":
                        _evt.Transactions.AddRange(ParseTransactionList(field)); break;
                    case "sourceList":
                        _evt.Sources.AddRange(ParseSourceList(field)); break;
                    case "destinationList":
                        _evt.Destinations.AddRange(ParseDestinationList(field)); break;
                    case "persistentDisposition":
                        _evt.PersistentDispositions.AddRange(ParsePersistentDisposition(field)); break;
                    case "sensorElementList":
                        _evt.SensorElements.AddRange(ParseSensorElements(field)); break;
                    case "ilmd":
                        ParseIlmd(field); break;
                    default:
                        throw new EpcisException(ExceptionType.ImplementationException, $"Unexpected event field: {field.Name}");
                }
            }
            else
            {
                ParseCustomFields(field, FieldType.CustomField, null, null);
            }
        }

        return _evt;
    }

    private void ParseReadPoint(XElement element)
    {
        foreach (var field in element.Elements())
        {
            if (field.Name.LocalName == "id")
            {
                _evt.ReadPoint = field.Value;
            }
            else
            {
                ParseCustomFields(field, FieldType.ReadPointCustomField, null, null); break;
            }
        }
    }

    private void ParseBizLocation(XElement element)
    {
        foreach (var field in element.Elements())
        {
            if (field.Name.LocalName == "id")
            {
                _evt.BusinessLocation = field.Value;
            }
            else
            {
                ParseCustomFields(field, FieldType.BusinessLocationCustomField, null, null); break;
            }
        }
    }

    private void ParseIlmd(XElement element)
    {
        foreach (var field in element.Elements())
        {
            ParseCustomFields(field, FieldType.Ilmd, null, null);
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
            Quantity = float.TryParse(x.Element("quantity")?.Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB"), out float quantity) ? quantity : default(float?),
            UnitOfMeasure = x.Element("uom")?.Value,
            Type = type,
        });
    }

    private static IEnumerable<BusinessTransaction> ParseTransactionList(XElement field)
    {
        return field.Elements().Select(x => new BusinessTransaction
        {
            Id = x.Value,
            Type = x.Attribute("type")?.Value ?? string.Empty
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

    private static IEnumerable<PersistentDisposition> ParsePersistentDisposition(XElement field)
    {
        return field.Elements().Select(x => new PersistentDisposition
        {
            Id = x.Value,
            Type = Enum.Parse<PersistentDispositionType>(x.Name.LocalName, true)
        });
    }

    public IEnumerable<SensorElement> ParseSensorElements(XElement field)
    {
        return field.Elements().Select(ParseSensorElement);
    }

    private SensorElement ParseSensorElement(XElement element)
    {
        var sensorElement = new SensorElement { Index = ++_index };

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
                ParseCustomFields(field, FieldType.Sensor, null, sensorElement.Index);
            }
        }

        return sensorElement;
    }

    private void ParseSensorReport(SensorElement sensorElement, XElement element)
    {
        var report = new SensorReport
        {
            Index = ++_index,
            SensorIndex = sensorElement.Index
        };

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
                        report.Time = DateTime.Parse(field.Value, null, DateTimeStyles.AdjustToUniversal); break;
                    case "meanValue":
                        report.MeanValue = float.Parse(field.Value); break;
                    case "percRank":
                        report.PercRank = float.Parse(field.Value); break;
                    case "percValue":
                        report.PercValue = float.Parse(field.Value); break;
                    case "dataProcessingMethod":
                        report.DataProcessingMethod = field.Value; break;
                    case "coordinateReferenceSystem":
                        report.CoordinateReferenceSystem = field.Value; break;
                    default:
                        throw new EpcisException(ExceptionType.ImplementationException, $"Unexpected event field: {field.Name}");
                }
            }
            else
            {
                ParseCustomFields(field, FieldType.SensorReport, null, report.Index);
            }
        }

        _evt.Reports.Add(report);
    }

    private void ParseSensorMetadata(SensorElement sensorElement, XElement metadata)
    {
        foreach (var field in metadata.Attributes())
        {
            if (string.IsNullOrEmpty(field.Name.NamespaceName))
            {
                switch (field.Name.LocalName)
                {
                    case "time":
                        sensorElement.Time = DateTime.Parse(field.Value, null, DateTimeStyles.AdjustToUniversal); break;
                    case "bizRules":
                        sensorElement.BizRules = field.Value; break;
                    case "deviceID":
                        sensorElement.DeviceId = field.Value; break;
                    case "deviceMetadata":
                        sensorElement.DeviceMetadata = field.Value; break;
                    case "rawData":
                        sensorElement.RawData = field.Value; break;
                    case "startTime":
                        sensorElement.StartTime = DateTime.Parse(field.Value, null, DateTimeStyles.AdjustToUniversal); break;
                    case "endTime":
                        sensorElement.EndTime = DateTime.Parse(field.Value, null, DateTimeStyles.AdjustToUniversal); break;
                    case "dataProcessingMethod":
                        sensorElement.DataProcessingMethod = field.Value; break;
                    default:
                        throw new EpcisException(ExceptionType.ImplementationException, $"Unexpected event field: {field.Name}");
                }
            }
            else
            {
                ParseCustomFields(field, FieldType.SensorMetadata, null, sensorElement.Index);
            }
        }
    }

    public void ParseCustomFields(XElement element, FieldType fieldType, int? parentIndex, int? entityIndex)
    {
        var field = new Field
        {
            Index = ++_index,
            ParentIndex = parentIndex,
            EntityIndex = entityIndex,
            Type = fieldType,
            Name = element.Name.LocalName,
            Namespace = string.IsNullOrWhiteSpace(element.Name.NamespaceName) ? default : element.Name.NamespaceName,
            TextValue = element.HasElements ? default : element.Value,
            NumericValue = element.HasElements ? default : float.TryParse(element.Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB"), out float floatValue) ? floatValue : default(float?),
            DateValue = element.HasElements ? default : DateTime.TryParse(element.Value, null, DateTimeStyles.AdjustToUniversal, out DateTime dateValue) ? dateValue : default(DateTime?)
        };

        foreach (var children in element.Elements())
        {
            ParseCustomFields(children, fieldType, field.Index, entityIndex);
        }
        foreach (var attribute in element.Attributes().Where(x => !x.IsNamespaceDeclaration))
        {
            ParseAttribute(attribute, field.Index, entityIndex);
        }

        _evt.Fields.Add(field);
    }

    public void ParseCustomFields(XAttribute element, FieldType fieldType, int? parentIndex, int? entityIndex)
    {
        _evt.Fields.Add(new()
        {
            Index = ++_index,
            ParentIndex = parentIndex,
            EntityIndex = entityIndex,
            Type = fieldType,
            Name = element.Name.LocalName,
            Namespace = string.IsNullOrWhiteSpace(element.Name.NamespaceName) ? default : element.Name.NamespaceName,
            TextValue = element.Value,
            NumericValue = float.TryParse(element.Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB"), out float floatValue) ? floatValue : default(float?),
            DateValue = DateTime.TryParse(element.Value, null, DateTimeStyles.AdjustToUniversal, out DateTime dateValue) ? dateValue : default(DateTime?)
        });
    }

    public void ParseAttribute(XAttribute element, int? parentIndex, int? entityIndex)
    {
        _evt.Fields.Add(new()
        {
            Index = ++_index,
            ParentIndex = parentIndex,
            EntityIndex = entityIndex,
            Type = FieldType.Attribute,
            Name = element.Name.LocalName,
            Namespace = element.Name.NamespaceName,
            TextValue = element.Value,
            NumericValue = float.TryParse(element.Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB"), out float floatValue) ? floatValue : default(float?),
            DateValue = DateTime.TryParse(element.Value, null, DateTimeStyles.AdjustToUniversal, out DateTime dateValue) ? dateValue : default(DateTime?)
        });
    }
}
