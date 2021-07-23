using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model;
using FasTnT.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace FasTnT.Formatter.Xml.Parsers
{
    public static class XmlEventParser
    {
        readonly static IDictionary<string, Func<XElement, Event>> RootParsers = new Dictionary<string, Func<XElement, Event>>
        {
            { "ObjectEvent", ParseObjectEvent },
            { "TransactionEvent", ParseTransactionEvent },
            { "AggregationEvent", ParseAggregationEvent },
            { "QuantityEvent", ParseQuantityEvent },
            { "extension", ParseEventListExtension }
        };

        readonly static IDictionary<string, Func<XElement, Event>> ExtensionParsers = new Dictionary<string, Func<XElement, Event>>
        {
            { "TransformationEvent", ParseTransformationEvent }
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

        public static Event ParseObjectEvent(XElement eventRoot)
        {
            var Event = ParseBase(eventRoot, EventType.Object);

            Event.Action = Enum.Parse<EventAction>(eventRoot.Element("action").Value, true);
            ParseTransactions(eventRoot.Element("bizTransactionList"), Event);
            ParseEpcList(eventRoot.Element("epcList"), Event, EpcType.List);
            ParseObjectExtension(eventRoot.Element("extension"), Event);

            return Event;
        }

        private static void ParseObjectExtension(XElement element, Event Event)
        {
            if (element == null || element.IsEmpty)
            {
                return;
            }

            ParseEpcQuantityList(element.Element("quantityList"), Event, EpcType.Quantity);
            ParseSources(element.Element("sourceList"), Event);
            ParseDestinations(element.Element("destinationList"), Event);
            ParseIlmd(element.Element("ilmd"), Event);
            ParseExtension(element.Element("extension"), Event, FieldType.Extension);
        }

        public static Event ParseAggregationEvent(XElement eventRoot)
        {
            var Event = ParseBase(eventRoot, EventType.Aggregation);

            Event.Action = Enum.Parse<EventAction>(eventRoot.Element("action").Value, true);
            ParseParentId(eventRoot.Element("parentID"), Event);
            ParseEpcList(eventRoot.Element("childEPCs"), Event, EpcType.ChildEpc);
            ParseTransactions(eventRoot.Element("bizTransactionList"), Event);
            ParseAggregationExtension(eventRoot.Element("extension"), Event);

            return Event;
        }

        private static void ParseAggregationExtension(XElement element, Event Event)
        {
            if (element == null || element.IsEmpty)
            {
                return;
            }

            ParseEpcQuantityList(element.Element("childQuantityList"), Event, EpcType.ChildQuantity);
            ParseSources(element.Element("sourceList"), Event);
            ParseDestinations(element.Element("destinationList"), Event);
            ParseExtension(element.Element("extension"), Event, FieldType.Extension);
        }

        public static Event ParseTransactionEvent(XElement eventRoot)
        {
            var Event = ParseBase(eventRoot, EventType.Transaction);

            Event.Action = Enum.Parse<EventAction>(eventRoot.Element("action").Value, true);
            ParseParentId(eventRoot.Element("parentID"), Event);
            ParseTransactions(eventRoot.Element("bizTransactionList"), Event);
            ParseEpcList(eventRoot.Element("epcList"), Event, EpcType.List);
            ParseTransactionExtension(eventRoot.Element("extension"), Event);

            return Event;
        }

        private static void ParseTransactionExtension(XElement element, Event Event)
        {
            if (element == null || element.IsEmpty)
            {
                return;
            }

            ParseEpcQuantityList(element.Element("quantityList"), Event, EpcType.Quantity);
            ParseSources(element.Element("sourceList"), Event);
            ParseDestinations(element.Element("destinationList"), Event);
            ParseExtension(element.Element("extension"), Event, FieldType.Extension);
        }

        public static Event ParseQuantityEvent(XElement eventRoot)
        {
            var Event = ParseBase(eventRoot, EventType.Quantity);
            var epcQuantity = new Epc
            {
                Id = eventRoot.Element("epcClass").Value,
                Quantity = float.Parse(eventRoot.Element("quantity").Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB")),
                IsQuantity = true,
                Type = EpcType.Quantity
            };

            Event.Epcs.Add(epcQuantity);
            ParseExtension(eventRoot.Element("extension"), Event, FieldType.Extension);

            return Event;
        }

        public static Event ParseTransformationEvent(XElement eventRoot)
        {
            var Event = ParseBase(eventRoot, EventType.Transformation);

            Event.TransformationId = eventRoot.Element("transformationID")?.Value;
            ParseTransactions(eventRoot.Element("bizTransactionList"), Event);
            ParseSources(eventRoot.Element("sourceList"), Event);
            ParseDestinations(eventRoot.Element("destinationList"), Event);
            ParseIlmd(eventRoot.Element("ilmd"), Event);
            ParseEpcList(eventRoot.Element("inputEPCList"), Event, EpcType.InputEpc);
            ParseEpcQuantityList(eventRoot.Element("inputQuantityList"), Event, EpcType.InputQuantity);
            ParseEpcList(eventRoot.Element("outputEPCList"), Event, EpcType.OutputEpc);
            ParseEpcQuantityList(eventRoot.Element("outputQuantityList"), Event, EpcType.OutputQuantity);
            ParseExtension(eventRoot.Element("extension"), Event, FieldType.Extension);

            return Event;
        }

        public static Event ParseBase(XElement eventRoot, EventType eventType)
        {
            var Event = new Event
            {
                Type = eventType,
                EventTime = DateTime.Parse(eventRoot.Element("eventTime").Value),
                EventTimeZoneOffset = new TimeZoneOffset { Representation = eventRoot.Element("eventTimeZoneOffset").Value },
                BusinessStep = eventRoot.Element("bizStep")?.Value,
                Disposition = eventRoot.Element("disposition")?.Value,
            };

            ParseReadPoint(eventRoot.Element("readPoint"), Event);
            ParseBusinessLocation(eventRoot.Element("bizLocation"), Event);
            ParseBaseExtension(eventRoot.Element("baseExtension"), Event);
            ParseFields(eventRoot, Event, FieldType.CustomField);

            return Event;
        }

        private static void ParseReadPoint(XElement readPoint, Event Event)
        {
            if (readPoint == null || readPoint.IsEmpty)
            {
                return;
            }

            Event.ReadPoint = readPoint.Element("id")?.Value;
            ParseExtension(readPoint.Element("extension"), Event, FieldType.ReadPointExtension);
            ParseFields(readPoint, Event, FieldType.ReadPointCustomField);
        }

        private static void ParseBusinessLocation(XElement bizLocation, Event Event)
        {
            if (bizLocation == null || bizLocation.IsEmpty)
            {
                return;
            }

            Event.BusinessLocation = bizLocation.Element("id")?.Value;
            ParseExtension(bizLocation.Element("extension"), Event, FieldType.BusinessLocationExtension);
            ParseFields(bizLocation, Event, FieldType.BusinessLocationCustomField);
        }

        private static void ParseParentId(XElement element, Event Event)
        {
            if (element == null || element.IsEmpty)
            {
                return;
            }

            Event.Epcs.Add(new Epc { Id = element.Value, Type = EpcType.ParentId });
        }

        private static void ParseIlmd(XElement element, Event Event)
        {
            if (element == null || element.IsEmpty)
            {
                return;
            }

            ParseFields(element, Event, FieldType.Ilmd);
            ParseExtension(element.Element("extension"), Event, FieldType.IlmdExtension);
        }

        private static void ParseEpcList(XElement element, Event Event, EpcType type)
        {
            if (element == null || element.IsEmpty)
            {
                return;
            }

            Event.Epcs.AddRange(element.Elements("epc").Select(x => new Epc { Id = x.Value, Type = type }));
        }

        private static void ParseEpcQuantityList(XElement element, Event Event, EpcType type)
        {
            if (element == null || element.IsEmpty)
            {
                return;
            }

            Event.Epcs.AddRange(element.Elements("quantityElement").Select(x => new Epc
            {
                Id = x.Element("epcClass").Value,
                IsQuantity = true,
                Quantity = float.TryParse(x.Element("quantity")?.Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB"), out float quantity) ? quantity : default(float?),
                UnitOfMeasure = x.Element("uom")?.Value,
                Type = type
            }));
        }

        private static void ParseBaseExtension(XElement element, Event Event)
        {
            if (element == null || element.IsEmpty)
            {
                return;
            }

            Event.EventId = element.Element("eventID")?.Value;
            ParseErrorDeclaration(element.Element("errorDeclaration"), Event);
            ParseExtension(element.Element("extension"), Event, FieldType.BaseExtension);
        }

        private static void ParseErrorDeclaration(XElement element, Event Event)
        {
            if (element == null || element.IsEmpty)
            {
                return;
            }

            Event.CorrectiveDeclarationTime = DateTime.Parse(element.Element("declarationTime").Value);
            Event.CorrectiveReason = element.Element("reason")?.Value;
            Event.CorrectiveEventIds.AddRange(element.Element("correctiveEventIDs")?.Elements("correctiveEventID")?.Select(x => new CorrectiveEventId { CorrectiveId = x.Value }));
            ParseExtension(element.Element("extension"), Event, FieldType.ErrorDeclarationExtension);
            ParseFields(element, Event, FieldType.ErrorDeclarationCustomField);
        }

        internal static void ParseFields(XElement element, Event Event, FieldType type)
        {
            if (element == null || element.IsEmpty)
            {
                return;
            }

            var customFields = element.Elements().Where(x => !string.IsNullOrEmpty(x.Name.NamespaceName));
            Event.CustomFields.AddRange(customFields.Select(x => ParseCustomFields(x, type)));
        }

        internal static void ParseExtension(XElement element, Event Event, FieldType type)
        {
            if (element == null || element.IsEmpty)
            {
                return;
            }

            var customFields = element.Elements().Where(x => string.IsNullOrEmpty(x.Name.NamespaceName));
            Event.CustomFields.AddRange(customFields.Select(x => ParseCustomFields(x, type)));
        }

        internal static void ParseSources(XElement element, Event Event)
        {
            if (element == null || element.IsEmpty)
            {
                return;
            }

            Event.Sources.AddRange(element.Elements("source").Select(CreateSource));
        }

        internal static void ParseDestinations(XElement element, Event Event)
        {
            if (element == null || element.IsEmpty)
            {
                return;
            }

            Event.Destinations.AddRange(element.Elements("destination").Select(CreateDest));
        }

        internal static void ParseTransactions(XElement element, Event Event)
        {
            if (element == null || element.IsEmpty)
            {
                return;
            }

            Event.Transactions.AddRange(element.Elements("bizTransaction").Select(CreateBusinessTransaction));
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

        public static CustomField ParseCustomFields(XElement element, FieldType fieldType)
        {
            var field = new CustomField
            {
                Type = fieldType,
                Name = element.Name.LocalName,
                Namespace = string.IsNullOrWhiteSpace(element.Name.NamespaceName) ? default : element.Name.NamespaceName,
                TextValue = element.HasElements ? default : element.Value,
                NumericValue = element.HasElements ? default : float.TryParse(element.Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB"), out float floatValue) ? floatValue : default(float?),
                DateValue = element.HasElements ? default : DateTime.TryParse(element.Value, out DateTime dateValue) ? dateValue : default(DateTime?)
            };

            field.Children.AddRange(element.Elements().Select(x => ParseCustomFields(x, fieldType)));
            field.Children.AddRange(element.Attributes().Where(x => !x.IsNamespaceDeclaration).Select(ParseAttribute));

            return field;
        }

        public static CustomField ParseAttribute(XAttribute element)
        {
            return new()
            {
                Type = FieldType.Attribute,
                Name = element.Name.LocalName,
                Namespace = element.Name.NamespaceName,
                TextValue = element.Value,
                NumericValue = float.TryParse(element.Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB"), out float floatValue) ? floatValue : default(float?),
                DateValue = DateTime.TryParse(element.Value, out DateTime dateValue) ? dateValue : default(DateTime?)
            };
        }
    }
}
