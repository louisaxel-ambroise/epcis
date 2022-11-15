using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model.Events;
using System.Text.Json;

namespace FasTnT.Features.v2_0.Communication.Json.Parsers;

public class JsonEventParser
{
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
            IsQuantity = true,
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
        return value.EnumerateArray().Select(x => JsonSensorElementParser.ParseSensorElement(x, _extensions));
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

    private IEnumerable<Field> ParseCustomField(JsonElement element, FieldType type, string propName, string propNs)
    {
        var field = new Field { Type = type, Name = propName, Namespace = propNs };

        if (element.ValueKind == JsonValueKind.Object)
        {
            field.Children.AddRange(element.EnumerateObject().SelectMany(e =>
            {
                var (ns, name) = ParseName(e.Name);
                return ParseCustomField(e.Value, type, name, ns);
            }));
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            return element.EnumerateArray().SelectMany(e => ParseCustomField(e, type, propName, propNs));
        }
        else
        {
            field.TextValue = element.GetString();
            field.NumericValue = float.TryParse(field.TextValue, out float numericValue) ? numericValue : default(float?);
            field.DateValue = DateTime.TryParse(field.TextValue, out DateTime dateValue) ? dateValue : default(DateTime?);
        }

        return new[] { field };
    }

    private (string Namespace, string Name) ParseName(string name)
    {
        var parts = name.Split(':', 2);

        return (_extensions[parts[0]], parts[1]);
    }
}
