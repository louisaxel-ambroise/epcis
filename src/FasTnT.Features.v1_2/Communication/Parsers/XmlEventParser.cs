using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model.Events;
namespace FasTnT.Features.v1_2.Communication.Parsers;

public static class XmlEventParser
{
    private readonly static Dictionary<string, Func<XElement, Event>> RootParsers = new()
    {
        { "ObjectEvent", ParseObjectEvent },
        { "TransactionEvent", ParseTransactionEvent },
        { "AggregationEvent", ParseAggregationEvent },
        { "QuantityEvent", ParseQuantityEvent },
        { "extension", ParseEventListExtension }
    };

    private readonly static Dictionary<string, Func<XElement, Event>> ExtensionParsers = new()
    {
        { "TransformationEvent", ParseTransformationEvent },
        { "extension", ParseEventListSubExtension }
    };

    private readonly static Dictionary<string, Func<XElement, Event>> SubExtensionParsers = new()
    {
        { "AssociationEvent", ParseAssociationEvent }
    };

    public static IEnumerable<Event> ParseEvents(XElement root)
    {
        foreach (var element in root.Elements())
        {
            if (!RootParsers.TryGetValue(element.Name.LocalName, out Func<XElement, Event> parser))
            {
                throw new ArgumentException($"Element '{element.Name.LocalName}' not expected in this context");
            }

            yield return parser(element);
        }
    }

    private static Event ParseEventListExtension(XElement element)
    {
        var eventElement = element.Elements().First();

        if (!ExtensionParsers.TryGetValue(eventElement.Name.LocalName, out Func<XElement, Event> parser))
        {
            throw new ArgumentException($"Element '{eventElement.Name.LocalName}' not expected in this context");
        }

        return parser(eventElement);
    }

    private static Event ParseEventListSubExtension(XElement element)
    {
        var eventElement = element.Elements().First();

        if (!SubExtensionParsers.TryGetValue(eventElement.Name.LocalName, out Func<XElement, Event> parser))
        {
            throw new ArgumentException($"Element '{eventElement.Name.LocalName}' not expected in this context");
        }

        return parser(eventElement);
    }

    public static Event ParseObjectEvent(XElement eventRoot)
    {
        var evt = ParseBase(eventRoot, EventType.ObjectEvent);

        evt.Action = Enum.Parse<EventAction>(eventRoot.Element("action").Value, true);
        ParseTransactions(eventRoot.Element("bizTransactionList"), evt);
        ParseEpcList(eventRoot.Element("epcList"), evt, EpcType.List);
        ParseObjectExtension(eventRoot.Element("extension"), evt);

        return evt;
    }

    private static void ParseObjectExtension(XElement element, Event evt)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        ParseEpcQuantityList(element.Element("quantityList"), evt, EpcType.Quantity);
        ParseSources(element.Element("sourceList"), evt);
        ParseDestinations(element.Element("destinationList"), evt);
        ParseIlmd(element.Element("ilmd"), evt);
        ParseV2Extensions(element.Element("extension"), evt);
    }

    public static Event ParseAggregationEvent(XElement eventRoot)
    {
        var evt = ParseBase(eventRoot, EventType.AggregationEvent);

        evt.Action = Enum.Parse<EventAction>(eventRoot.Element("action").Value, true);
        ParseParentId(eventRoot.Element("parentID"), evt);
        ParseEpcList(eventRoot.Element("childEPCs"), evt, EpcType.ChildEpc);
        ParseTransactions(eventRoot.Element("bizTransactionList"), evt);
        ParseAggregationExtension(eventRoot.Element("extension"), evt);

        return evt;
    }

    private static void ParseAggregationExtension(XElement element, Event evt)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        ParseEpcQuantityList(element.Element("childQuantityList"), evt, EpcType.ChildQuantity);
        ParseSources(element.Element("sourceList"), evt);
        ParseDestinations(element.Element("destinationList"), evt);
        ParseV2Extensions(element.Element("extension"), evt);
    }

    public static Event ParseTransactionEvent(XElement eventRoot)
    {
        var evt = ParseBase(eventRoot, EventType.TransactionEvent);

        evt.Action = Enum.Parse<EventAction>(eventRoot.Element("action").Value, true);
        ParseParentId(eventRoot.Element("parentID"), evt);
        ParseTransactions(eventRoot.Element("bizTransactionList"), evt);
        ParseEpcList(eventRoot.Element("epcList"), evt, EpcType.List);
        ParseTransactionExtension(eventRoot.Element("extension"), evt);

        return evt;
    }

    private static void ParseTransactionExtension(XElement element, Event evt)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        ParseEpcQuantityList(element.Element("quantityList"), evt, EpcType.Quantity);
        ParseSources(element.Element("sourceList"), evt);
        ParseDestinations(element.Element("destinationList"), evt);
        ParseV2Extensions(element.Element("extension"), evt);
    }

    public static Event ParseQuantityEvent(XElement eventRoot)
    {
        var evt = ParseBase(eventRoot, EventType.QuantityEvent);
        var epcQuantity = new Epc
        {
            Id = eventRoot.Element("epcClass").Value,
            Quantity = float.Parse(eventRoot.Element("quantity").Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB")),
            Type = EpcType.Quantity
        };

        evt.Epcs.Add(epcQuantity);
        ParseExtension(eventRoot.Element("extension"), evt, FieldType.Extension);

        return evt;
    }

    public static Event ParseTransformationEvent(XElement eventRoot)
    {
        var evt = ParseBase(eventRoot, EventType.TransformationEvent);

        evt.TransformationId = eventRoot.Element("transformationID")?.Value;
        ParseTransactions(eventRoot.Element("bizTransactionList"), evt);
        ParseSources(eventRoot.Element("sourceList"), evt);
        ParseDestinations(eventRoot.Element("destinationList"), evt);
        ParseIlmd(eventRoot.Element("ilmd"), evt);
        ParseEpcList(eventRoot.Element("inputEPCList"), evt, EpcType.InputEpc);
        ParseEpcQuantityList(eventRoot.Element("inputQuantityList"), evt, EpcType.InputQuantity);
        ParseEpcList(eventRoot.Element("outputEPCList"), evt, EpcType.OutputEpc);
        ParseEpcQuantityList(eventRoot.Element("outputQuantityList"), evt, EpcType.OutputQuantity);
        ParseV2Extensions(eventRoot.Element("extension"), evt);

        return evt;
    }

    public static Event ParseAssociationEvent(XElement eventRoot)
    {
        var evt = ParseBase(eventRoot, EventType.AssociationEvent);

        evt.Action = Enum.Parse<EventAction>(eventRoot.Element("action").Value, true);
        ParseParentId(eventRoot.Element("parentID"), evt);
        ParseEpcList(eventRoot.Element("childEPCs"), evt, EpcType.ChildEpc);
        ParseTransactions(eventRoot.Element("bizTransactionList"), evt);
        ParseAssociationExtension(eventRoot.Element("extension"), evt);

        return evt;
    }

    private static void ParseAssociationExtension(XElement element, Event evt)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        ParseEpcQuantityList(element.Element("childQuantityList"), evt, EpcType.ChildQuantity);
        ParseSources(element.Element("sourceList"), evt);
        ParseDestinations(element.Element("destinationList"), evt);
        ParseV2Extensions(element.Element("extension"), evt);
    }

    public static Event ParseBase(XElement eventRoot, EventType eventType)
    {
        var Event = new Event
        {
            Type = eventType,
            EventTime = DateTimeOffset.Parse(eventRoot.Element("eventTime").Value, null, DateTimeStyles.AdjustToUniversal),
            EventTimeZoneOffset = eventRoot.Element("eventTimeZoneOffset").Value,
            BusinessStep = eventRoot.Element("bizStep")?.Value,
            Disposition = eventRoot.Element("disposition")?.Value,
        };

        ParseReadPoint(eventRoot.Element("readPoint"), Event);
        ParseBusinessLocation(eventRoot.Element("bizLocation"), Event);
        ParseBaseExtension(eventRoot.Element("baseExtension"), Event);
        ParseFields(eventRoot, Event, FieldType.CustomField);

        return Event;
    }

    private static void ParseReadPoint(XElement readPoint, Event evt)
    {
        if (readPoint == null || readPoint.IsEmpty)
        {
            return;
        }

        evt.ReadPoint = readPoint.Element("id")?.Value;
        ParseExtension(readPoint.Element("extension"), evt, FieldType.ReadPointExtension);
        ParseFields(readPoint, evt, FieldType.ReadPointCustomField);
    }

    private static void ParseBusinessLocation(XElement bizLocation, Event evt)
    {
        if (bizLocation == null || bizLocation.IsEmpty)
        {
            return;
        }

        evt.BusinessLocation = bizLocation.Element("id")?.Value;
        ParseExtension(bizLocation.Element("extension"), evt, FieldType.BusinessLocationExtension);
        ParseFields(bizLocation, evt, FieldType.BusinessLocationCustomField);
    }

    private static void ParseParentId(XElement element, Event evt)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        evt.Epcs.Add(new Epc { Id = element.Value, Type = EpcType.ParentId });
    }

    private static void ParseIlmd(XElement element, Event evt)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        ParseFields(element, evt, FieldType.Ilmd);
        ParseExtension(element.Element("extension"), evt, FieldType.IlmdExtension);
    }

    private static void ParseEpcList(XElement element, Event evt, EpcType type)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        evt.Epcs.AddRange(element.Elements("epc").Select(x => new Epc { Id = x.Value, Type = type }));
    }

    private static void ParseEpcQuantityList(XElement element, Event evt, EpcType type)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        evt.Epcs.AddRange(element.Elements("quantityElement").Select(x => new Epc
        {
            Id = x.Element("epcClass").Value,
            Quantity = float.TryParse(x.Element("quantity")?.Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB"), out float quantity) ? quantity : default(float?),
            UnitOfMeasure = x.Element("uom")?.Value,
            Type = type
        }));
    }

    private static void ParseBaseExtension(XElement element, Event evt)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        evt.EventId = element.Element("eventID")?.Value;
        ParseErrorDeclaration(element.Element("errorDeclaration"), evt);
        ParseExtension(element.Element("extension"), evt, FieldType.BaseExtension);
    }

    private static void ParseV2Extensions(XElement element, Event evt)
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
                    evt.PersistentDispositions.AddRange(XmlExtensionParser.ParsePersistentDisposition(field)); break;
                case "sensorElementList":
                    evt.SensorElements.AddRange(XmlExtensionParser.ParseSensorList(field)); break;
                default:
                    evt.Fields.Add(XmlCustomFieldParser.ParseCustomFields(field, FieldType.Extension)); break;
            }
        }

        ParseExtension(element.Element("extension"), evt, FieldType.Extension);
    }

    private static void ParseErrorDeclaration(XElement element, Event evt)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        evt.CorrectiveDeclarationTime = DateTimeOffset.Parse(element.Element("declarationTime").Value, null, DateTimeStyles.AdjustToUniversal);
        evt.CorrectiveReason = element.Element("reason")?.Value;
        evt.CorrectiveEventIds.AddRange(element.Element("correctiveEventIDs")?.Elements("correctiveEventID")?.Select(x => new CorrectiveEventId { CorrectiveId = x.Value }));
        ParseExtension(element.Element("extension"), evt, FieldType.ErrorDeclarationExtension);
        ParseFields(element, evt, FieldType.ErrorDeclarationCustomField);
    }

    internal static void ParseFields(XElement element, Event Event, FieldType type)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        var customFields = element.Elements().Where(x => !string.IsNullOrEmpty(x.Name.NamespaceName));
        Event.Fields.AddRange(customFields.Select(x => XmlCustomFieldParser.ParseCustomFields(x, type)));
    }

    internal static void ParseExtension(XElement element, Event evt, FieldType type)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        var customFields = element.Elements().Where(x => string.IsNullOrEmpty(x.Name.NamespaceName));
        evt.Fields.AddRange(customFields.Select(x => XmlCustomFieldParser.ParseCustomFields(x, type)));
    }

    internal static void ParseSources(XElement element, Event evt)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        evt.Sources.AddRange(element.Elements("source").Select(CreateSource));
    }

    internal static void ParseDestinations(XElement element, Event evt)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        evt.Destinations.AddRange(element.Elements("destination").Select(CreateDest));
    }

    internal static void ParseTransactions(XElement element, Event evt)
    {
        if (element == null || element.IsEmpty)
        {
            return;
        }

        evt.Transactions.AddRange(element.Elements("bizTransaction").Select(CreateBusinessTransaction));
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
}
