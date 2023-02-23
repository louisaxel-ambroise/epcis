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
        foreach (var element in root.Elements())
        {
            var parser = new XmlEventParser();

            switch (element.Name.LocalName)
            {
                case "ObjectEvent":
                    parser.ParseObjectEvent(element); break;
                case "TransactionEvent":
                    parser.ParseTransactionEvent(element); break;
                case "AggregationEvent":
                    parser.ParseAggregationEvent(element); break;
                case "QuantityEvent":
                    parser.ParseQuantityEvent(element); break;
                case "extension":
                    parser.ParseEventListExtension(element); break;
                default:
                    throw new ArgumentException($"Element '{element.Name.LocalName}' not expected in this context");
            }

            yield return parser._evt;
        }
    }

    private Event ParseEventListExtension(XElement element)
    {
        var eventElement = element.Elements().First();

        return eventElement.Name.LocalName switch
        {
            "TransformationEvent" => ParseTransformationEvent(eventElement),
            "extension" => ParseEventListSubExtension(eventElement),
            _ => throw new ArgumentException($"Element '{element.Name.LocalName}' not expected in this context")
        };
    }

    private Event ParseEventListSubExtension(XElement element)
    {
        var eventElement = element.Elements().First();

        return eventElement.Name.LocalName == "AssociationEvent" 
            ? ParseAssociationEvent(eventElement)
            : throw new ArgumentException($"Element '{element.Name.LocalName}' not expected in this context");
    }

    public Event ParseObjectEvent(XElement eventRoot)
    {
        ParseBase(eventRoot, EventType.ObjectEvent);
        ParseTransactions(eventRoot.Element("bizTransactionList"));
        ParseEpcList(eventRoot.Element("epcList"), EpcType.List);
        ParseObjectExtension(eventRoot.Element("extension"));

        _evt.Action = Enum.Parse<EventAction>(eventRoot.Element("action").Value, true);

        return _evt;
    }

    private void ParseObjectExtension(XElement element)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        ParseEpcQuantityList(element.Element("quantityList"), EpcType.Quantity);
        ParseSources(element.Element("sourceList"));
        ParseDestinations(element.Element("destinationList"));
        ParseIlmd(element.Element("ilmd"));
        ParseV2Extensions(element.Element("extension"));
    }

    public Event ParseAggregationEvent(XElement eventRoot)
    {
        ParseBase(eventRoot, EventType.AggregationEvent);
        ParseParentId(eventRoot.Element("parentID"));
        ParseEpcList(eventRoot.Element("childEPCs"), EpcType.ChildEpc);
        ParseTransactions(eventRoot.Element("bizTransactionList"));
        ParseAggregationExtension(eventRoot.Element("extension"));

        _evt.Action = Enum.Parse<EventAction>(eventRoot.Element("action").Value, true);

        return _evt;
    }

    private void ParseAggregationExtension(XElement element)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        ParseEpcQuantityList(element.Element("childQuantityList"), EpcType.ChildQuantity);
        ParseSources(element.Element("sourceList"));
        ParseDestinations(element.Element("destinationList"));
        ParseV2Extensions(element.Element("extension"));
    }

    public Event ParseTransactionEvent(XElement eventRoot)
    {
        ParseBase(eventRoot, EventType.TransactionEvent);
        ParseParentId(eventRoot.Element("parentID"));
        ParseTransactions(eventRoot.Element("bizTransactionList"));
        ParseEpcList(eventRoot.Element("epcList"), EpcType.List);
        ParseTransactionExtension(eventRoot.Element("extension"));

        _evt.Action = Enum.Parse<EventAction>(eventRoot.Element("action").Value, true);

        return _evt;
    }

    private void ParseTransactionExtension(XElement element)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        ParseEpcQuantityList(element.Element("quantityList"), EpcType.Quantity);
        ParseSources(element.Element("sourceList"));
        ParseDestinations(element.Element("destinationList"));
        ParseV2Extensions(element.Element("extension"));
    }

    public Event ParseQuantityEvent(XElement eventRoot)
    {
        ParseBase(eventRoot, EventType.QuantityEvent);
        ParseExtension(eventRoot.Element("extension"), FieldType.Extension);

        _evt.Epcs.Add(new Epc
        {
            Id = eventRoot.Element("epcClass").Value,
            Quantity = float.Parse(eventRoot.Element("quantity").Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB")),
            Type = EpcType.Quantity
        });

        return _evt;
    }

    public Event ParseTransformationEvent(XElement eventRoot)
    {
        ParseBase(eventRoot, EventType.TransformationEvent);
        ParseTransactions(eventRoot.Element("bizTransactionList"));
        ParseSources(eventRoot.Element("sourceList"));
        ParseDestinations(eventRoot.Element("destinationList"));
        ParseIlmd(eventRoot.Element("ilmd"));
        ParseEpcList(eventRoot.Element("inputEPCList"), EpcType.InputEpc);
        ParseEpcQuantityList(eventRoot.Element("inputQuantityList"), EpcType.InputQuantity);
        ParseEpcList(eventRoot.Element("outputEPCList"), EpcType.OutputEpc);
        ParseEpcQuantityList(eventRoot.Element("outputQuantityList"), EpcType.OutputQuantity);
        ParseV2Extensions(eventRoot.Element("extension"));

        _evt.TransformationId = eventRoot.Element("transformationID")?.Value;

        return _evt;
    }

    public Event ParseAssociationEvent(XElement eventRoot)
    {
        ParseBase(eventRoot, EventType.AssociationEvent);
        ParseParentId(eventRoot.Element("parentID"));
        ParseEpcList(eventRoot.Element("childEPCs"), EpcType.ChildEpc);
        ParseTransactions(eventRoot.Element("bizTransactionList"));
        ParseAssociationExtension(eventRoot.Element("extension"));

        _evt.Action = Enum.Parse<EventAction>(eventRoot.Element("action").Value, true);

        return _evt;
    }

    private void ParseAssociationExtension(XElement element)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        ParseEpcQuantityList(element.Element("childQuantityList"), EpcType.ChildQuantity);
        ParseSources(element.Element("sourceList"));
        ParseDestinations(element.Element("destinationList"));
        ParseV2Extensions(element.Element("extension"));
    }

    public void ParseBase(XElement eventRoot, EventType eventType)
    {
        _evt = new Event
        {
            Type = eventType,
            EventTime = DateTime.Parse(eventRoot.Element("eventTime").Value, null, DateTimeStyles.AdjustToUniversal),
            EventTimeZoneOffset = eventRoot.Element("eventTimeZoneOffset").Value,
            BusinessStep = eventRoot.Element("bizStep")?.Value,
            Disposition = eventRoot.Element("disposition")?.Value,
        };

        ParseReadPoint(eventRoot.Element("readPoint"));
        ParseBusinessLocation(eventRoot.Element("bizLocation"));
        ParseBaseExtension(eventRoot.Element("baseExtension"));
        ParseFields(eventRoot, FieldType.CustomField);
    }

    private void ParseReadPoint(XElement readPoint)
    {
        if (readPoint == null || readPoint.IsEmpty)
        {
            return;
        }

        _evt.ReadPoint = readPoint.Element("id")?.Value;
        ParseExtension(readPoint.Element("extension"), FieldType.ReadPointExtension);
        ParseFields(readPoint, FieldType.ReadPointCustomField);
    }

    private void ParseBusinessLocation(XElement bizLocation)
    {
        if (bizLocation == null || bizLocation.IsEmpty)
        {
            return;
        }

        _evt.BusinessLocation = bizLocation.Element("id")?.Value;
        ParseExtension(bizLocation.Element("extension"), FieldType.BusinessLocationExtension);
        ParseFields(bizLocation, FieldType.BusinessLocationCustomField);
    }

    private void ParseParentId(XElement element)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        _evt.Epcs.Add(new Epc { Id = element.Value, Type = EpcType.ParentId });
    }

    private void ParseIlmd(XElement element)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        ParseFields(element, FieldType.Ilmd);
        ParseExtension(element.Element("extension"), FieldType.IlmdExtension);
    }

    private void ParseEpcList(XElement element, EpcType type)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        _evt.Epcs.AddRange(element.Elements("epc").Select(x => new Epc { Id = x.Value, Type = type }));
    }

    private void ParseEpcQuantityList(XElement element, EpcType type)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        _evt.Epcs.AddRange(element.Elements("quantityElement").Select(x => new Epc
        {
            Id = x.Element("epcClass").Value,
            Quantity = float.TryParse(x.Element("quantity")?.Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB"), out float quantity) ? quantity : default(float?),
            UnitOfMeasure = x.Element("uom")?.Value,
            Type = type
        }));
    }

    private void ParseBaseExtension(XElement element)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        _evt.EventId = element.Element("eventID")?.Value;
        ParseErrorDeclaration(element.Element("errorDeclaration"));
        ParseExtension(element.Element("extension"), FieldType.BaseExtension);
    }

    private void ParseV2Extensions(XElement element)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        foreach (var field in element.Elements())
        {
            switch (field.Name.LocalName)
            {
                case "persistentDisposition":
                    _evt.PersistentDispositions.AddRange(ParsePersistentDisposition(field)); break;
                case "sensorElementList":
                    _evt.SensorElements.AddRange(ParseSensorList(field)); break;
                default:
                    _evt.Fields.AddRange(ParseCustomFields(field, FieldType.Extension, null)); break;
            }
        }

        ParseExtension(element.Element("extension"), FieldType.Extension);
    }

    private void ParseErrorDeclaration(XElement element)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        _evt.CorrectiveDeclarationTime = DateTime.Parse(element.Element("declarationTime").Value, null, DateTimeStyles.AdjustToUniversal);
        _evt.CorrectiveReason = element.Element("reason")?.Value;
        _evt.CorrectiveEventIds.AddRange(element.Element("correctiveEventIDs")?.Elements("correctiveEventID")?.Select(x => new CorrectiveEventId { CorrectiveId = x.Value }));
        ParseExtension(element.Element("extension"), FieldType.ErrorDeclarationExtension);
        ParseFields(element, FieldType.ErrorDeclarationCustomField);
    }

    internal void ParseFields(XElement element, FieldType type)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        var customFields = element.Elements().Where(x => !string.IsNullOrEmpty(x.Name.NamespaceName));
        _evt.Fields.AddRange(customFields.SelectMany(x => ParseCustomFields(x, type, null)));
    }

    internal void ParseExtension(XElement element, FieldType type)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        var customFields = element.Elements().Where(x => string.IsNullOrEmpty(x.Name.NamespaceName));
        _evt.Fields.AddRange(customFields.SelectMany(x => ParseCustomFields(x, type, null)));
    }

    internal void ParseSources(XElement element)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        _evt.Sources.AddRange(element.Elements("source").Select(CreateSource));
    }

    internal void ParseDestinations(XElement element)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        _evt.Destinations.AddRange(element.Elements("destination").Select(CreateDest));
    }

    internal void ParseTransactions(XElement element)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        _evt.Transactions.AddRange(element.Elements("bizTransaction").Select(CreateBusinessTransaction));
    }

    private static BusinessTransaction CreateBusinessTransaction(XElement element)
    {
        return new()
        {
            Id = element.Value,
            Type = element.Attribute("type").Value
        };
    }

    private static Source CreateSource(XElement element)
    {
        return new() { Type = element.Attribute("type").Value, Id = element.Value };
    }

    private static Destination CreateDest(XElement element)
    {
        return new() { Type = element.Attribute("type").Value, Id = element.Value };
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
                    ParseSensorReport(sensorElement, field); break;
                }
            }
            else
            {
                _evt.Fields.AddRange(ParseCustomFields(field, FieldType.Sensor, null));
            }
        }

        return sensorElement;
    }

    private void ParseSensorReport(SensorElement sensorElement, XElement element)
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
                        report.Time = DateTime.Parse(field.Value, null, DateTimeStyles.AdjustToUniversal); break;
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
                _evt.Fields.Add(ParseCustomFields(field, FieldType.SensorReport));
            }
        }

        sensorElement.Reports.Add(report);
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
                _evt.Fields.Add(ParseCustomFields(field, FieldType.SensorMetadata));
            }
        }
    }

    public IEnumerable<Field> ParseCustomFields(XElement element, FieldType fieldType, int? parentIndex)
    {
        var parsed = new List<Field>();
        var field = new Field
        {
            Index = ++_index,
            ParentIndex = parentIndex,
            Type = fieldType,
            Name = element.Name.LocalName,
            Namespace = string.IsNullOrWhiteSpace(element.Name.NamespaceName) ? default : element.Name.NamespaceName,
            TextValue = element.HasElements ? default : element.Value,
            NumericValue = element.HasElements ? default : float.TryParse(element.Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB"), out float floatValue) ? floatValue : default(float?),
            DateValue = element.HasElements ? default : DateTime.TryParse(element.Value, null, DateTimeStyles.AdjustToUniversal, out DateTime dateValue) ? dateValue : default(DateTime?)
        };

        parsed.Add(field);
        parsed.AddRange(element.Elements().SelectMany(x => ParseCustomFields(x, fieldType, field.Index)));
        parsed.AddRange(element.Attributes().Where(x => !x.IsNamespaceDeclaration).Select(x => ParseAttribute(x, field.Index)));

        return parsed;
    }

    public Field ParseCustomFields(XAttribute element, FieldType fieldType)
    {
        return new()
        {
            Index = ++_index,
            Type = fieldType,
            Name = element.Name.LocalName,
            Namespace = string.IsNullOrWhiteSpace(element.Name.NamespaceName) ? default : element.Name.NamespaceName,
            TextValue = element.Value,
            NumericValue = float.TryParse(element.Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB"), out float floatValue) ? floatValue : default(float?),
            DateValue = DateTime.TryParse(element.Value, null, DateTimeStyles.AdjustToUniversal, out DateTime dateValue) ? dateValue : default(DateTime?)
        };
    }

    public Field ParseAttribute(XAttribute element, int parentIndex)
    {
        return new()
        {
            Index = ++_index,
            ParentIndex = parentIndex,
            Type = FieldType.Attribute,
            Name = element.Name.LocalName,
            Namespace = element.Name.NamespaceName,
            TextValue = element.Value,
            NumericValue = float.TryParse(element.Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB"), out float floatValue) ? floatValue : default(float?),
            DateValue = DateTime.TryParse(element.Value, null, DateTimeStyles.AdjustToUniversal, out DateTime dateValue) ? dateValue : default(DateTime?)
        };
    }
}
