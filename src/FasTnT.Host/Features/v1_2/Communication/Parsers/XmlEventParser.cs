using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Events;

namespace FasTnT.Host.Features.v1_2.Communication.Parsers;

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

        switch (element.Name.LocalName)
        {
            case "QuantityEvent":
                parser.ParseQuantityEvent(element); break;
            case "ObjectEvent":
                parser.ParseEvent(element, EventType.ObjectEvent); break;
            case "TransactionEvent":
                parser.ParseEvent(element, EventType.TransactionEvent); break;
            case "AggregationEvent":
                parser.ParseEvent(element, EventType.AggregationEvent); break;
            case "extension":
                ParseEventListExtension(parser, element); break;
            default:
                throw new ArgumentException($"Element '{element.Name.LocalName}' not expected in this context");
        }

        return parser._evt;
    }

    private static void ParseEventListExtension(XmlEventParser parser, XElement element)
    {
        var eventElement = element.Elements().First();

        switch (eventElement.Name.LocalName)
        {
            case "TransformationEvent":
                parser.ParseEvent(eventElement, EventType.TransformationEvent); break;
            case "extension":
                ParseEventListSubExtension(parser, eventElement); break;
            default:
                throw new ArgumentException($"Element '{eventElement.Name.LocalName}' not expected in this context");
        }
    }

    private static void ParseEventListSubExtension(XmlEventParser parser, XElement element)
    {
        var eventElement = element.Elements().First();

        if (eventElement.Name.LocalName == "AssociationEvent")
        {
            parser.ParseEvent(eventElement, EventType.AssociationEvent);
        }
        else
        {
            throw new ArgumentException($"Element '{eventElement.Name.LocalName}' not expected in this context");
        }
    }

    private void ParseQuantityEvent(XElement element)
    {
        ParseEvent(element, EventType.QuantityEvent);

        _evt.Epcs.Add(new Epc
        {
            Id = element.Element("epcClass").Value,
            Quantity = float.TryParse(element.Element("quantity")?.Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB"), out var quantity) ? quantity : default,
            Type = EpcType.Quantity
        });
    }

    private void ParseEvent(XElement element, EventType eventType)
    {
        _evt = new Event { Type = eventType };

        foreach (var field in element.Elements())
        {
            if (string.IsNullOrEmpty(field.Name.NamespaceName))
            {
                switch (field.Name.LocalName)
                {
                    case "action":
                        _evt.Action = Enum.Parse<EventAction>(field.Value, true); break;
                    case "recordTime": // Discard - this will be overridden
                    case "epcClass": // These fields are reserved for the (deprecated) Quantity event. Ignore them.
                    case "quantity":
                        break;
                    case "eventTime":
                        _evt.EventTime = UtcDateTime.Parse(field.Value); break;
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
                        ParseBusinessLocation(field); break;
                    case "eventID":
                        _evt.EventId = field.Value; break;
                    case "baseExtension":
                        ParseBaseExtension(field); break;
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
                        _evt.Epcs.AddRange(ParseEpcQuantityList(field, EpcType.Quantity)); break;
                    case "childQuantityList":
                        _evt.Epcs.AddRange(ParseEpcQuantityList(field, EpcType.ChildQuantity)); break;
                    case "inputQuantityList":
                        _evt.Epcs.AddRange(ParseEpcQuantityList(field, EpcType.InputQuantity)); break;
                    case "outputQuantityList":
                        _evt.Epcs.AddRange(ParseEpcQuantityList(field, EpcType.OutputQuantity)); break;
                    case "bizTransactionList":
                        _evt.Transactions.AddRange(ParseTransactionList(field)); break;
                    case "sourceList":
                        _evt.Sources.AddRange(ParseSources(field)); break;
                    case "destinationList":
                        _evt.Destinations.AddRange(ParseDestinations(field)); break;
                    case "ilmd":
                        ParseFields(field, FieldType.Ilmd); break;
                    case "extension":
                        ParseEventExtension(field); break;
                    default:
                        throw new EpcisException(ExceptionType.ImplementationException, $"Unexpected event field: {field.Name}");
                }
            }
            else
            {
                ParseCustomFields(field, FieldType.CustomField, null, null);
            }
        }
    }

    private void ParseEventExtension(XElement element)
    {
        foreach (var field in element.Elements())
        {
            if (string.IsNullOrEmpty(field.Name.NamespaceName))
            {
                switch (field.Name.LocalName)
                {
                    case "childEPCs":
                        _evt.Epcs.AddRange(ParseEpcList(field, EpcType.ChildEpc)); break;
                    case "inputEPCList":
                        _evt.Epcs.AddRange(ParseEpcList(field, EpcType.InputEpc)); break;
                    case "outputEPCList":
                        _evt.Epcs.AddRange(ParseEpcList(field, EpcType.OutputEpc)); break;
                    case "quantityList":
                        _evt.Epcs.AddRange(ParseEpcQuantityList(field, EpcType.Quantity)); break;
                    case "childQuantityList":
                        _evt.Epcs.AddRange(ParseEpcQuantityList(field, EpcType.ChildQuantity)); break;
                    case "inputQuantityList":
                        _evt.Epcs.AddRange(ParseEpcQuantityList(field, EpcType.InputQuantity)); break;
                    case "outputQuantityList":
                        _evt.Epcs.AddRange(ParseEpcQuantityList(field, EpcType.OutputQuantity)); break;
                    case "bizTransactionList":
                        _evt.Transactions.AddRange(ParseTransactionList(field)); break;
                    case "sourceList":
                        _evt.Sources.AddRange(ParseSources(field)); break;
                    case "destinationList":
                        _evt.Destinations.AddRange(ParseDestinations(field)); break;
                    case "ilmd":
                        ParseFields(field, FieldType.Ilmd); break;
                    case "persistentDisposition":
                        _evt.PersistentDispositions.AddRange(ParsePersistentDisposition(field)); break;
                    case "sensorElementList":
                        _evt.SensorElements.AddRange(ParseSensorList(field)); break;
                    case "extension":
                        ParseEventExtension(field); break;
                    default:
                        throw new EpcisException(ExceptionType.ImplementationException, $"Unexpected event field: {field.Name}");
                }
            }
            else
            {
                ParseCustomFields(field, FieldType.Extension, null, null);
            }
        }
    }

    private void ParseBaseExtension(XElement element)
    {
        foreach (var field in element.Elements())
        {
            if (string.IsNullOrEmpty(field.Name.NamespaceName))
            {
                switch (field.Name.LocalName)
                {
                    case "eventID":
                        _evt.EventId = field.Value; break;
                    case "errorDeclaration":
                        ParseErrorDeclaration(field); break;
                    case "extension":
                        ParseFields(field, FieldType.BaseExtension); break;
                    default:
                        throw new EpcisException(ExceptionType.ImplementationException, $"Unexpected event field: {field.Name}");
                }
            }
            else
            {
                ParseCustomFields(field, FieldType.BaseExtension, null, null);
            }
        }
    }

    private void ParseReadPoint(XElement readPoint)
    {
        foreach (var field in readPoint.Elements())
        {
            if (string.IsNullOrEmpty(field.Name.NamespaceName))
            {
                switch (field.Name.LocalName)
                {
                    case "id":
                        _evt.ReadPoint = field.Value; break;
                    case "extension":
                        ParseFields(field, FieldType.ReadPointExtension); break;
                    default:
                        throw new EpcisException(ExceptionType.ImplementationException, $"Unexpected event field: {field.Name}");
                }
            }
            else
            {
                ParseCustomFields(field, FieldType.ReadPointCustomField, null, null);
            }
        }
    }

    private void ParseBusinessLocation(XElement bizLocation)
    {
        foreach (var field in bizLocation.Elements())
        {
            if (string.IsNullOrEmpty(field.Name.NamespaceName))
            {
                switch (field.Name.LocalName)
                {
                    case "id":
                        _evt.BusinessLocation = field.Value; break;
                    case "extension":
                        ParseFields(field, FieldType.BusinessLocationExtension); break;
                    default:
                        throw new EpcisException(ExceptionType.ImplementationException, $"Unexpected event field: {field.Name}");
                }
            }
            else
            {
                ParseCustomFields(field, FieldType.BusinessLocationCustomField, null, null);
            }
        }
    }

    private static IEnumerable<Epc> ParseEpcList(XElement element, EpcType type)
    {
        return element.Elements("epc").Select(x => new Epc { Id = x.Value, Type = type });
    }

    private static IEnumerable<Epc> ParseEpcQuantityList(XElement element, EpcType type)
    {
        return element.Elements("quantityElement").Select(x => new Epc
        {
            Id = x.Element("epcClass").Value,
            Quantity = float.TryParse(x.Element("quantity")?.Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB"), out float quantity) ? quantity : default(float?),
            UnitOfMeasure = x.Element("uom")?.Value,
            Type = type
        });
    }

    private void ParseErrorDeclaration(XElement element)
    {
        foreach (var field in element.Elements())
        {
            if (string.IsNullOrEmpty(field.Name.NamespaceName))
            {
                switch (field.Name.LocalName)
                {
                    case "declarationTime":
                        _evt.CorrectiveDeclarationTime = UtcDateTime.Parse(field.Value); break;
                    case "reason":
                        _evt.CorrectiveReason = field.Value; break;
                    case "correctiveEventIDs":
                        _evt.CorrectiveEventIds.AddRange(field.Elements("correctiveEventID").Select(x => new CorrectiveEventId { CorrectiveId = x.Value })); break;
                    case "extension":
                        ParseFields(field, FieldType.ErrorDeclarationExtension); break;
                    default:
                        throw new EpcisException(ExceptionType.ImplementationException, $"Unexpected event field: {field.Name}");
                }
            }
            else
            {
                ParseCustomFields(field, FieldType.ErrorDeclarationCustomField, null, null);
            }
        }
    }

    internal static IEnumerable<Source> ParseSources(XElement element)
    {
        return element.Elements("source").Select(x => new Source
        {
            Type = x.Attribute("type").Value,
            Id = x.Value
        });
    }

    internal static IEnumerable<Destination> ParseDestinations(XElement element)
    {
        return element.Elements("destination").Select(x => new Destination
        {
            Type = x.Attribute("type").Value,
            Id = x.Value
        });
    }

    internal static IEnumerable<BusinessTransaction> ParseTransactionList(XElement element)
    {
        return element.Elements("bizTransaction").Select(x => new BusinessTransaction
        {
            Id = x.Value,
            Type = x.Attribute("type")?.Value ?? string.Empty
        });
    }

    public static IEnumerable<PersistentDisposition> ParsePersistentDisposition(XElement field)
    {
        return field.Elements().Select(x => new PersistentDisposition
        {
            Id = x.Value,
            Type = Enum.Parse<PersistentDispositionType>(x.Name.LocalName, true)
        });
    }

    public IEnumerable<SensorElement> ParseSensorList(XElement field)
    {
        return field.Elements().Select(ParseSensorElement);
    }

    private SensorElement ParseSensorElement(XElement element)
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
                    _evt.Reports.Add(ParseSensorReport(sensorElement, field)); break;
                }
            }
            else
            {
                ParseCustomFields(field, FieldType.Sensor, null, null);
            }
        }

        return sensorElement;
    }

    private SensorReport ParseSensorReport(SensorElement sensorElement, XElement element)
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
                        report.Time = UtcDateTime.Parse(field.Value); break;
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
                ParseCustomFields(field, FieldType.SensorReport, null, null);
            }
        }

        return report;
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
                        sensorElement.Time = UtcDateTime.Parse(field.Value); break;
                    case "bizRules":
                        sensorElement.BizRules = field.Value; break;
                    case "deviceID":
                        sensorElement.DeviceId = field.Value; break;
                    case "deviceMetadata":
                        sensorElement.DeviceMetadata = field.Value; break;
                    case "rawData":
                        sensorElement.RawData = field.Value; break;
                    case "startTime":
                        sensorElement.StartTime = UtcDateTime.Parse(field.Value); break;
                    case "endTime":
                        sensorElement.EndTime = UtcDateTime.Parse(field.Value); break;
                    case "dataProcessingMethod":
                        sensorElement.DataProcessingMethod = field.Value; break;
                    default:
                        throw new EpcisException(ExceptionType.ImplementationException, $"Unexpected event field: {field.Name}");
                }
            }
            else
            {
                ParseCustomFields(field, FieldType.SensorMetadata, null, null);
            }
        }
    }

    private void ParseFields(XElement element, FieldType fieldType)
    {
        foreach (var field in element.Elements())
        {
            ParseCustomFields(field, fieldType, null, null);
        }
    }

    private void ParseCustomFields(XElement element, FieldType fieldType, int? parentIndex, int? entityIndex)
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
            DateValue = element.HasElements ? default : UtcDateTime.TryParse(element.Value, out DateTime dateValue) ? dateValue : default(DateTime?)
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

    private void ParseCustomFields(XAttribute element, FieldType fieldType, int? parentIndex, int? entityIndex)
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
            DateValue = UtcDateTime.TryParse(element.Value, out DateTime dateValue) ? dateValue : default(DateTime?)
        });
    }

    private void ParseAttribute(XAttribute element, int? parentIndex, int? entityIndex)
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
            DateValue = UtcDateTime.TryParse(element.Value, out DateTime dateValue) ? dateValue : default(DateTime?)
        });
    }
}
