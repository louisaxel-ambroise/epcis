using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model.Events;
using LinqKit;
using System.Text.Json;

namespace FasTnT.Host.Features.v2_0.Communication.Json.Parsers;

public class JsonEventParser
{
    private int _index;
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
        var evt = new Event();

        foreach (var property in _element.EnumerateObject())
        {
            switch (property.Name)
            {
                case "eventID":
                    evt.EventId = property.Value.GetString(); break;
                case "type":
                    evt.Type = Enum.Parse<EventType>(property.Value.GetString(), true); break;
                case "action":
                    evt.Action = Enum.Parse<EventAction>(property.Value.GetString(), true); break;
                case "parentID":
                    evt.Epcs.Add(new Epc { Type = EpcType.ParentId, Id = property.Value.GetString() }); break;
                case "certificationInfo":
                    evt.CertificationInfo = ParseCertificationInfo(property.Value); break;
                case "epcList":
                    evt.Epcs.AddRange(ParseEpcList(property.Value, EpcType.List)); break;
                case "childEPCs":
                    evt.Epcs.AddRange(ParseEpcList(property.Value, EpcType.ChildEpc)); break;
                case "inputEPCList":
                    evt.Epcs.AddRange(ParseEpcList(property.Value, EpcType.InputEpc)); break;
                case "outputEPCList":
                    evt.Epcs.AddRange(ParseEpcList(property.Value, EpcType.OutputEpc)); break;
                case "quantityList":
                    evt.Epcs.AddRange(ParseQuantityList(property.Value, EpcType.Quantity)); break;
                case "inputQuantityList":
                    evt.Epcs.AddRange(ParseQuantityList(property.Value, EpcType.InputQuantity)); break;
                case "outputQuantityList":
                    evt.Epcs.AddRange(ParseQuantityList(property.Value, EpcType.OutputQuantity)); break;
                case "childQuantityList":
                    evt.Epcs.AddRange(ParseQuantityList(property.Value, EpcType.ChildQuantity)); break;
                case "bizStep":
                    evt.BusinessStep = property.Value.GetString(); break;
                case "transformationID":
                    evt.TransformationId = property.Value.GetString(); break;
                case "disposition":
                    evt.Disposition = property.Value.GetString(); break;
                case "eventTime":
                    evt.EventTime = property.Value.GetDateTime().ToUniversalTime(); break;
                case "eventTimeZoneOffset":
                    evt.EventTimeZoneOffset = property.Value.GetString(); break;
                case "readPoint":
                    evt.ReadPoint = ParseIdElement(property.Value); break;
                case "bizLocation":
                    evt.BusinessLocation = ParseIdElement(property.Value); break;
                case "sourceList":
                    evt.Sources.AddRange(ParseSourceList(property.Value)); break;
                case "destinationList":
                    evt.Destinations.AddRange(ParseDestinationList(property.Value)); break;
                case "bizTransactionList":
                    evt.Transactions.AddRange(ParseBusinessTransactions(property.Value)); break;
                case "sensorElementList":
                    evt.SensorElements.AddRange(ParseSensorElements(property.Value)); break;
                case "persistentDisposition":
                    evt.PersistentDispositions.AddRange(ParsePersistentDispositions(property.Value)); break;
                case "ilmd":
                    evt.Fields.AddRange(ParseIlmd(property)); break;
                case "recordTime":
                /* Don't do anything - record time is set to the time the event was inserted. */
                case "@context":
                    /* Don't do anything - context was already parsed. */
                    break;
                default:
                    evt.Fields.AddRange(ParseCustomField(property)); break;
            }
        }

        return evt;
    }

    private static string ParseIdElement(JsonElement element) => element.GetProperty("id").GetString();

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
            Type = x.GetProperty("type").GetString()
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
            var (ns, name) = ParseName(e.Name);
            return ParseCustomField(e.Value, FieldType.Ilmd, name, ns);
        });
    }

    private IEnumerable<Field> ParseCustomField(JsonProperty jsonProperty)
    {
        var (ns, name) = ParseName(jsonProperty.Name);
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
                    sensorElement.Reports.AddRange(ParseSensorReports(property.Value)); break;
                default:
                    ParseCustomField(property, FieldType.Sensor, null, sensorElement.Index); break;
            }
        }

        return sensorElement;
    }

    private IEnumerable<SensorReport> ParseSensorReports(JsonElement element)
    {
        return element.EnumerateArray().Select(ParseSensorReport);
    }

    private SensorReport ParseSensorReport(JsonElement element)
    {
        var report = new SensorReport { Index = ++_index };

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
                    report.SensorElement.Event.Fields.AddRange(ParseCustomField(property, FieldType.SensorReport, null, report.Index)); break;
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
                    sensorElement.Event.Fields.AddRange(ParseCustomField(property, FieldType.SensorMetadata, null, sensorElement.Index)); break;
            }
        }
    }

    private IEnumerable<Field> ParseCustomField(JsonProperty jsonProperty, FieldType type, int? parentIndex = null, int? entityIndex = null)
    {
        var (ns, name) = ParseName(jsonProperty.Name);
        return ParseCustomField(jsonProperty.Value, type, name, ns, parentIndex, entityIndex);
    }

    private IEnumerable<Field> ParseCustomField(JsonElement element, FieldType type, string propName, string propNs, int? parentIndex = null, int? entityIndex = null)
    {
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
                var (ns, name) = ParseName(e.Name);
                var fields = ParseCustomField(e.Value, type, name, ns, field.Index, entityIndex);
                fields.ForEach(x => x.ParentIndex = field.Index);

                return fields;
            }));
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            return element.EnumerateArray().SelectMany(e => ParseCustomField(e, type, propName, propNs, field.Index, entityIndex));
        }
        else
        {
            field.TextValue = element.GetString();
            field.NumericValue = float.TryParse(field.TextValue, out float numericValue) ? numericValue : default(float?);
            field.DateValue = DateTime.TryParse(field.TextValue, null, DateTimeStyles.AdjustToUniversal, out DateTime dateValue) ? dateValue : default(DateTime?);
        }

        return customFields;
    }

    private (string Namespace, string Name) ParseName(string name)
    {
        var parts = name.Split(':', 2);

        return (_extensions[parts[0]], parts[1]);
    }
}
