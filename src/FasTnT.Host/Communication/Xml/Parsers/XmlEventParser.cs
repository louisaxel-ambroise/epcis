using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Events;

namespace FasTnT.Host.Communication.Xml.Parsers;

public abstract class XmlEventParser
{
    internal int Index;
    internal Event Event;

    public IEnumerable<Event> ParseEvents(XElement root)
    {
        return root.Elements().Select(ParseEvent);
    }

    public abstract Event ParseEvent(XElement eventElement);

    internal void ParseReadPoint(XElement readPoint)
    {
        foreach (var field in readPoint.Elements())
        {
            if (string.IsNullOrEmpty(field.Name.NamespaceName))
            {
                switch (field.Name.LocalName)
                {
                    case "id":
                        Event.ReadPoint = field.Value; break;
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

    internal void ParseBusinessLocation(XElement bizLocation)
    {
        foreach (var field in bizLocation.Elements())
        {
            if (string.IsNullOrEmpty(field.Name.NamespaceName))
            {
                switch (field.Name.LocalName)
                {
                    case "id":
                        Event.BusinessLocation = field.Value; break;
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

    internal static IEnumerable<Epc> ParseEpcList(XElement element, EpcType type)
    {
        return element.Elements("epc").Select(x => new Epc { Id = x.Value, Type = type });
    }

    internal static IEnumerable<Epc> ParseEpcQuantityList(XElement element, EpcType type)
    {
        return element.Elements("quantityElement").Select(x => new Epc
        {
            Id = x.Element("epcClass").Value,
            Quantity = float.TryParse(x.Element("quantity")?.Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB"), out float quantity) ? quantity : default(float?),
            UnitOfMeasure = x.Element("uom")?.Value,
            Type = type
        });
    }

    internal void ParseErrorDeclaration(XElement element)
    {
        foreach (var field in element.Elements())
        {
            if (string.IsNullOrEmpty(field.Name.NamespaceName))
            {
                switch (field.Name.LocalName)
                {
                    case "declarationTime":
                        Event.CorrectiveDeclarationTime = DateTime.Parse(field.Value, null, DateTimeStyles.AdjustToUniversal); break;
                    case "reason":
                        Event.CorrectiveReason = field.Value; break;
                    case "correctiveEventIDs":
                        Event.CorrectiveEventIds.AddRange(field.Elements("correctiveEventID").Select(x => new CorrectiveEventId { CorrectiveId = x.Value })); break;
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

    internal static IEnumerable<PersistentDisposition> ParsePersistentDisposition(XElement field)
    {
        return field.Elements().Select(x => new PersistentDisposition
        {
            Id = x.Value,
            Type = Enum.Parse<PersistentDispositionType>(x.Name.LocalName, true)
        });
    }

    internal IEnumerable<SensorElement> ParseSensorList(XElement field)
    {
        return field.Elements().Select(ParseSensorElement);
    }

    internal SensorElement ParseSensorElement(XElement element)
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
                    Event.Reports.Add(ParseSensorReport(sensorElement, field)); break;
                }
            }
            else
            {
                ParseCustomFields(field, FieldType.Sensor, null, null);
            }
        }

        return sensorElement;
    }

    internal SensorReport ParseSensorReport(SensorElement sensorElement, XElement element)
    {
        var report = new SensorReport
        {
            Index = ++Index,
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
                ParseCustomFields(field, FieldType.SensorReport, null, null);
            }
        }

        return report;
    }

    internal void ParseSensorMetadata(SensorElement sensorElement, XElement metadata)
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
                ParseCustomFields(field, FieldType.SensorMetadata, null, null);
            }
        }
    }

    internal void ParseFields(XElement element, FieldType fieldType)
    {
        foreach (var field in element.Elements())
        {
            ParseCustomFields(field, fieldType, null, null);
        }
    }

    internal void ParseCustomFields(XElement element, FieldType fieldType, int? parentIndex, int? entityIndex)
    {
        var field = new Field
        {
            Index = ++Index,
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

        Event.Fields.Add(field);
    }

    internal void ParseCustomFields(XAttribute element, FieldType fieldType, int? parentIndex, int? entityIndex)
    {
        Event.Fields.Add(new()
        {
            Index = ++Index,
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

    internal void ParseAttribute(XAttribute element, int? parentIndex, int? entityIndex)
    {
        Event.Fields.Add(new()
        {
            Index = ++Index,
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
