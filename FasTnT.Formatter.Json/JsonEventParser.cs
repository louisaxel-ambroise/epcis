using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace FasTnT.Formatter.Json;

public class JsonEventParser
{
    private readonly JsonElement _element;
    private readonly JsonCustomFieldParser _customFieldParser;

    internal JsonEventParser(JsonElement element, IDictionary<string, string> extensions)
    {
        _element = element;
        _customFieldParser = new(extensions);
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
                case "isA":
                    evt.Type = Enum.Parse<EventType>(property.Value.GetString()); break;
                case "action":
                    evt.Action = Enum.Parse<EventAction>(property.Value.GetString()); break;
                case "parentID":
                    evt.Epcs.Add(new Epc { Type = EpcType.ParentId, Id = property.Value.GetString() }); break;
                case "epcList":
                    evt.Epcs.AddRange(ParseEpcList(property.Value, EpcType.List)); break;
                case "childEPCs":
                    evt.Epcs.AddRange(ParseEpcList(property.Value, EpcType.ChildEpc)); break;
                case "inputEPCList":
                    evt.Epcs.AddRange(ParseEpcList(property.Value, EpcType.InputEpc)); break;
                case "outputEPCList":
                    evt.Epcs.AddRange(ParseEpcList(property.Value, EpcType.OutputEpc)); break;
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
                    evt.EventTime = property.Value.GetDateTime(); break;
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
                    evt.CustomFields.AddRange(_customFieldParser.ParseIlmd(property)); break;
                case "recordTime":
                    /* Don't do anything - record time is set to the time the event was inserted. */
                    break;
                default:
                    evt.CustomFields.Add(_customFieldParser.ParseCustomField(property)); break;
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

    // TODO: parse
    private static IEnumerable<SensorElement> ParseSensorElements(JsonElement value)
    {
        return value.EnumerateArray().Select(JsonSensorElementParser.ParseSensorElement);
    }
}

class JsonCustomFieldParser
{
    private readonly IDictionary<string, string> _extensions;

    public JsonCustomFieldParser(IDictionary<string, string> extensions)
    {
        _extensions = extensions;
    }

    public IEnumerable<CustomField> ParseIlmd(JsonProperty jsonProperty)
    {
        return jsonProperty.Value.EnumerateObject().Select(e => 
        {
            var (ns, name) = ParseName(e.Name);
            return ParseCustomField(e.Value, FieldType.Ilmd, name, ns);
        });
    }

    public CustomField ParseCustomField(JsonProperty jsonProperty)
    {
        var (ns, name) = ParseName(jsonProperty.Name);
        return ParseCustomField(jsonProperty.Value, FieldType.CustomField, name, ns);
    }

    private CustomField ParseCustomField(JsonElement element, FieldType type, string propName, string propNs)
    {
        var field = new CustomField { Type = type, Name = propName, Namespace = propNs };

        if (element.ValueKind == JsonValueKind.Object)
        {
            field.Children.AddRange(element.EnumerateObject().Select(e =>
            {
                var (ns, name) = ParseName(e.Name);
                return ParseCustomField(e.Value, type, name, ns);
            }));
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            field.Children.AddRange(element.EnumerateArray().Select(e => ParseCustomField(e, type, propName, propNs)));
        }
        else
        {
            field.TextValue = element.GetString();
            field.NumericValue = float.TryParse(field.TextValue, out float numericValue) ? numericValue : default(float?);
            field.DateValue = DateTime.TryParse(field.TextValue, out DateTime dateValue) ? dateValue : default(DateTime?);
        }

        return field;
    }

    private (string Namespace, string Name) ParseName(string name)
    {
        var parts = name.Split(':', 2);

        return (_extensions[parts[0]], parts[1]);
    }
}
