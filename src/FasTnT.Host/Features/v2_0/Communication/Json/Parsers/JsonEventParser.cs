using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Events;
using System.Text.Json;

namespace FasTnT.Host.Features.v2_0.Communication.Json.Parsers;

public class JsonEventParser
{
    private int _index;
    private readonly Event _evt = new();
    private readonly JsonElement _element;
    private readonly Namespaces _extensions;

    private JsonEventParser(JsonElement element, Namespaces extensions)
    {
        _element = element;
        _extensions = extensions;
    }

    public static JsonEventParser Create(JsonElement element, Namespaces extensions)
    {
        if (element.TryGetProperty("@context", out var context))
        {
            extensions = extensions.Merge(Namespaces.Parse(context));
        }

        return new(element, extensions);
    }

    public Event Parse()
    {
        foreach (var property in _element.EnumerateObject())
        {
            switch (property.Name)
            {
                case "eventID":
                    _evt.EventId = property.Value.GetString(); break;
                case "type":
                    _evt.Type = Enum.Parse<EventType>(property.Value.GetString(), true); break;
                case "action":
                    _evt.Action = Enum.Parse<EventAction>(property.Value.GetString(), true); break;
                case "parentID":
                    _evt.Epcs.Add(new Epc { Type = EpcType.ParentId, Id = property.Value.GetString() }); break;
                case "certificationInfo":
                    _evt.CertificationInfo = ParseCertificationInfo(property.Value); break;
                case "errorDeclaration":
                    ParseErrorDeclaration(property); break;
                case "epcList":
                    _evt.Epcs.AddRange(ParseEpcList(property.Value, EpcType.List)); break;
                case "childEPCs":
                    _evt.Epcs.AddRange(ParseEpcList(property.Value, EpcType.ChildEpc)); break;
                case "inputEPCList":
                    _evt.Epcs.AddRange(ParseEpcList(property.Value, EpcType.InputEpc)); break;
                case "outputEPCList":
                    _evt.Epcs.AddRange(ParseEpcList(property.Value, EpcType.OutputEpc)); break;
                case "quantityList":
                    _evt.Epcs.AddRange(ParseQuantityList(property.Value, EpcType.Quantity)); break;
                case "inputQuantityList":
                    _evt.Epcs.AddRange(ParseQuantityList(property.Value, EpcType.InputQuantity)); break;
                case "outputQuantityList":
                    _evt.Epcs.AddRange(ParseQuantityList(property.Value, EpcType.OutputQuantity)); break;
                case "childQuantityList":
                    _evt.Epcs.AddRange(ParseQuantityList(property.Value, EpcType.ChildQuantity)); break;
                case "bizStep":
                    _evt.BusinessStep = property.Value.GetString(); break;
                case "transformationID":
                    _evt.TransformationId = property.Value.GetString(); break;
                case "disposition":
                    _evt.Disposition = property.Value.GetString(); break;
                case "eventTime":
                    _evt.EventTime = property.Value.GetDateTime().ToUniversalTime(); break;
                case "eventTimeZoneOffset":
                    _evt.EventTimeZoneOffset = property.Value.GetString(); break;
                case "readPoint":
                    ParseReadPoint(property); break;
                case "bizLocation":
                    ParseBizLocation(property); break;
                case "sourceList":
                    _evt.Sources.AddRange(ParseSourceList(property.Value)); break;
                case "destinationList":
                    _evt.Destinations.AddRange(ParseDestinationList(property.Value)); break;
                case "bizTransactionList":
                    _evt.Transactions.AddRange(ParseBusinessTransactions(property.Value)); break;
                case "sensorElementList":
                    _evt.SensorElements.AddRange(ParseSensorElements(property.Value)); break;
                case "persistentDisposition":
                    _evt.PersistentDispositions.AddRange(ParsePersistentDispositions(property.Value)); break;
                case "ilmd":
                    _evt.Fields.AddRange(ParseIlmd(property)); break;
                case "recordTime":
                    /* Don't do anything - record time is set to the time the event was inserted. */
                case "@context":
                    /* Don't do anything - context was already parsed. */
                    break;
                default:
                    _evt.Fields.AddRange(ParseCustomField(property)); break;
            }
        }

        return _evt;
    }

    private void ParseErrorDeclaration(JsonProperty errorDeclaration)
    {
        foreach(var property in errorDeclaration.Value.EnumerateObject())
        {
            switch (property.Name)
            {
                case "declarationTime":
                    _evt.CorrectiveDeclarationTime = property.Value.GetDateTime().ToUniversalTime(); break;
                case "reason":
                    _evt.CorrectiveReason = property.Value.GetString();break;
                case "correctiveEventIDs":
                    _evt.CorrectiveEventIds.AddRange(property.Value.EnumerateArray().Select(elt => new CorrectiveEventId { CorrectiveId = elt.GetString() })); break;
                default:
                    throw new EpcisException(ExceptionType.ImplementationException, $"Unexpected field: {property.Name}");
            }
        }
    }

    private void ParseReadPoint(JsonProperty readPoint)
    {
        foreach(var property in readPoint.Value.EnumerateObject())
        {
            switch (property.Name)
            {
                case "id":
                    _evt.ReadPoint = property.Value.GetString(); break;
                default:
                    _evt.Fields.AddRange(ParseCustomField(property, FieldType.ReadPointCustomField)); break;
            }
        }
    }

    private void ParseBizLocation(JsonProperty bizLocation)
    {
        foreach(var property in bizLocation.Value.EnumerateObject())
        {
            switch (property.Name)
            {
                case "id":
                    _evt.BusinessLocation = property.Value.GetString(); break;
                default:
                    _evt.Fields.AddRange(ParseCustomField(property, FieldType.BusinessLocationCustomField)); break;
            }
        }
    }

    private static IEnumerable<Epc> ParseEpcList(JsonElement element, EpcType type)
    {
        return element.EnumerateArray().Select(x => new Epc
        {
            Type = type,
            Id = x.GetString(),
        });
    }

    private static IEnumerable<Epc> ParseQuantityList(JsonElement element, EpcType type)
    {
        return element.EnumerateArray().Select(x => new Epc
        {
            Type = type,
            Id = x.GetProperty("epcClass").GetString(),
            Quantity = x.TryGetProperty("quantity", out var quantity) ? quantity.GetSingle() : null,
            UnitOfMeasure = x.TryGetProperty("uom", out var uom) ? uom.GetString() : null,
        });
    }

    private static IEnumerable<BusinessTransaction> ParseBusinessTransactions(JsonElement element)
    {
        return element.EnumerateArray().Select(x => new BusinessTransaction
        {
            Id = x.GetProperty("bizTransaction").GetString(),
            Type = x.TryGetProperty("type", out var txType) ? txType.GetString() : string.Empty
        });
    }

    private static IEnumerable<Source> ParseSourceList(JsonElement element)
    {
        return element.EnumerateArray().Select(x => new Source
        {
            Id = x.GetProperty("source").GetString(),
            Type = x.GetProperty("type").GetString()
        });
    }

    private static IEnumerable<Destination> ParseDestinationList(JsonElement element)
    {
        return element.EnumerateArray().Select(x => new Destination
        {
            Id = x.GetProperty("destination").GetString(),
            Type = x.GetProperty("type").GetString()
        });
    }

    private static IEnumerable<PersistentDisposition> ParsePersistentDispositions(JsonElement value)
    {
        var parser = (JsonProperty p) => p.Value.EnumerateArray().Select(v => new PersistentDisposition
        {
            Type = Enum.Parse<PersistentDispositionType>(p.Name, true),
            Id = v.GetString()
        });

        return value.EnumerateObject().SelectMany(parser);
    }

    private static string ParseCertificationInfo(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.Array => value.EnumerateArray().Select(x => x.GetString()).Single(),
            JsonValueKind.String => value.GetString(),
            _ => throw new Exception("Invalid CertificationInfo value")
        };
    }

    private IEnumerable<SensorElement> ParseSensorElements(JsonElement value)
    {
        return value.EnumerateArray().Select(ParseSensorElement);
    }

    private IEnumerable<Field> ParseIlmd(JsonProperty jsonProperty)
    {
        return jsonProperty.Value.EnumerateObject().SelectMany(e =>
        {
            var (ns, name) = _extensions.ParseName(e.Name);
            return ParseCustomField(e.Value, FieldType.Ilmd, name, ns);
        });
    }

    private IEnumerable<Field> ParseCustomField(JsonProperty jsonProperty)
    {
        var (ns, name) = _extensions.ParseName(jsonProperty.Name);
        return ParseCustomField(jsonProperty.Value, FieldType.CustomField, name, ns);
    }

    public SensorElement ParseSensorElement(JsonElement element)
    {
        var sensorElement = new SensorElement { Index = ++_index };

        foreach (var property in element.EnumerateObject())
        {
            switch (property.Name)
            {
                case "type":
                    break; // Can it be something different from epcis:SensorElement?
                case "sensorMetadata":
                    ParseSensorMetadata(sensorElement, property.Value); break;
                case "sensorReport":
                    _evt.Reports.AddRange(ParseSensorReports(property.Value, sensorElement.Index)); break;
                default:
                    ParseCustomField(property, FieldType.Sensor, null, sensorElement.Index); break;
            }
        }

        return sensorElement;
    }

    private IEnumerable<SensorReport> ParseSensorReports(JsonElement element, int sensorIndex)
    {
        return element.EnumerateArray().Select(x => ParseSensorReport(x, sensorIndex));
    }

    private SensorReport ParseSensorReport(JsonElement element, int sensorIndex)
    {
        var report = new SensorReport
        {
            Index = ++_index,
            SensorIndex = sensorIndex
        };

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
                    report.Time = property.Value.GetDateTime().ToUniversalTime(); break;
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
                    report.MinValue = property.Value.GetSingle(); break;
                case "maxValue":
                    report.MaxValue = property.Value.GetSingle(); break;
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
                case "coordinateReferenceSystem":
                    report.CoordinateReferenceSystem = property.Value.GetString(); break;
                default:
                    _evt.Fields.AddRange(ParseCustomField(property, FieldType.SensorReport, null, report.Index)); break;
            }
        }

        return report;
    }

    private void ParseSensorMetadata(SensorElement sensorElement, JsonElement element)
    {
        foreach (var property in element.EnumerateObject())
        {
            switch (property.Name)
            {
                case "time":
                    sensorElement.Time = property.Value.GetDateTime().ToUniversalTime(); break;
                case "deviceID":
                    sensorElement.DeviceId = property.Value.GetString(); break;
                case "deviceMetadata":
                    sensorElement.DeviceMetadata = property.Value.GetString(); break;
                case "rawData":
                    sensorElement.RawData = property.Value.GetString(); break;
                case "startTime":
                    sensorElement.StartTime = property.Value.GetDateTime().ToUniversalTime(); break;
                case "endTime":
                    sensorElement.EndTime = property.Value.GetDateTime().ToUniversalTime(); break;
                case "dataProcessingMethod":
                    sensorElement.DataProcessingMethod = property.Value.GetString(); break;
                case "bizRules":
                    sensorElement.BizRules = property.Value.GetString(); break;
                default:
                    _evt.Fields.AddRange(ParseCustomField(property, FieldType.SensorMetadata, null, sensorElement.Index)); break;
            }
        }
    }

    private IEnumerable<Field> ParseCustomField(JsonProperty jsonProperty, FieldType type, int? parentIndex = null, int? entityIndex = null)
    {
        var (ns, name) = _extensions.ParseName(jsonProperty.Name);
        return ParseCustomField(jsonProperty.Value, type, name, ns, parentIndex, entityIndex);
    }

    private IEnumerable<Field> ParseCustomField(JsonElement element, FieldType type, string propName, string propNs, int? parentIndex = null, int? entityIndex = null)
    {
        if (element.ValueKind == JsonValueKind.Array)
        {
            return element
                .EnumerateArray()
                .SelectMany(e => ParseCustomField(e, type, propName, propNs, parentIndex, entityIndex));
        }

        var customFields = new List<Field>();
        var field = new Field
        {
            Type = type,
            Name = propName,
            Namespace = propNs,
            Index = ++_index,
            ParentIndex = parentIndex,
            EntityIndex = entityIndex
        };

        customFields.Add(field);

        if (element.ValueKind == JsonValueKind.Object)
        {
            customFields.AddRange(element.EnumerateObject().SelectMany(e =>
            {
                var (ns, name) = _extensions.ParseName(e.Name);
                
                return ParseCustomField(e.Value, type, name, ns, field.Index, entityIndex);
            }));
        }
        else
        {
            field.TextValue = element.GetString();
            field.NumericValue = float.TryParse(field.TextValue, out float numericValue) ? numericValue : default(float?);
            field.DateValue = DateTime.TryParse(field.TextValue, null, DateTimeStyles.AdjustToUniversal, out DateTime dateValue) ? dateValue : default(DateTime?);
        }

        return customFields;
    }
}
