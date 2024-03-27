using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Events;

namespace FasTnT.Host.Communication.Xml.Parsers;

public class XmlV1EventParser : XmlEventParser
{
    public override Event ParseEvent(XElement element)
    {
        Index = 0;

        switch (element.Name.LocalName)
        {
            case "QuantityEvent":
                ParseQuantityEvent(element); break;
            case "ObjectEvent":
                ParseEvent(element, EventType.ObjectEvent); break;
            case "TransactionEvent":
                ParseEvent(element, EventType.TransactionEvent); break;
            case "AggregationEvent":
                ParseEvent(element, EventType.AggregationEvent); break;
            case "extension":
                ParseEventListExtension(element); break;
            default:
                throw new ArgumentException($"Element '{element.Name.LocalName}' not expected in this context");
        }

        return Event;
    }

    private void ParseEventListExtension(XElement element)
    {
        var eventElement = element.Elements().First();

        switch (eventElement.Name.LocalName)
        {
            case "TransformationEvent":
                ParseEvent(eventElement, EventType.TransformationEvent); break;
            case "extension":
                ParseEventListSubExtension(eventElement); break;
            default:
                throw new ArgumentException($"Element '{eventElement.Name.LocalName}' not expected in this context");
        }
    }

    private void ParseEventListSubExtension(XElement element)
    {
        var eventElement = element.Elements().First();

        if (eventElement.Name.LocalName == "AssociationEvent")
        {
            ParseEvent(eventElement, EventType.AssociationEvent);
        }
        else
        {
            throw new ArgumentException($"Element '{eventElement.Name.LocalName}' not expected in this context");
        }
    }

    private void ParseQuantityEvent(XElement element)
    {
        ParseEvent(element, EventType.QuantityEvent);

        Event.Epcs.Add(new Epc
        {
            Id = element.Element("epcClass").Value,
            Quantity = float.TryParse(element.Element("quantity")?.Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB"), out var quantity) ? quantity : default,
            Type = EpcType.Quantity
        });
    }

    private void ParseEvent(XElement element, EventType eventType)
    {
        Event = new Event { Type = eventType };

        foreach (var field in element.Elements())
        {
            if (string.IsNullOrEmpty(field.Name.NamespaceName))
            {
                switch (field.Name.LocalName)
                {
                    case "action":
                        Event.Action = Enum.Parse<EventAction>(field.Value, true); break;
                    case "recordTime": // Discard - this will be overridden
                    case "epcClass": // These fields are reserved for the (deprecated) Quantity event. Ignore them.
                    case "quantity":
                        break;
                    case "eventTime":
                        Event.EventTime = DateTime.Parse(field.Value, null, DateTimeStyles.AdjustToUniversal); break;
                    case "certificationInfo":
                        Event.CertificationInfo = field.Value; break;
                    case "eventTimeZoneOffset":
                        Event.EventTimeZoneOffset = field.Value; break;
                    case "bizStep":
                        Event.BusinessStep = field.Value; break;
                    case "disposition":
                        Event.Disposition = field.Value; break;
                    case "transformationID":
                        Event.TransformationId = field.Value; break;
                    case "readPoint":
                        ParseReadPoint(field); break;
                    case "bizLocation":
                        ParseBusinessLocation(field); break;
                    case "eventID":
                        Event.EventId = field.Value; break;
                    case "baseExtension":
                        ParseBaseExtension(field); break;
                    case "parentID":
                        Event.Epcs.Add(new Epc { Type = EpcType.ParentId, Id = field.Value }); break;
                    case "epcList":
                        Event.Epcs.AddRange(ParseEpcList(field, EpcType.List)); break;
                    case "childEPCs":
                        Event.Epcs.AddRange(ParseEpcList(field, EpcType.ChildEpc)); break;
                    case "inputEPCList":
                        Event.Epcs.AddRange(ParseEpcList(field, EpcType.InputEpc)); break;
                    case "outputEPCList":
                        Event.Epcs.AddRange(ParseEpcList(field, EpcType.OutputEpc)); break;
                    case "quantityList":
                        Event.Epcs.AddRange(ParseEpcQuantityList(field, EpcType.Quantity)); break;
                    case "childQuantityList":
                        Event.Epcs.AddRange(ParseEpcQuantityList(field, EpcType.ChildQuantity)); break;
                    case "inputQuantityList":
                        Event.Epcs.AddRange(ParseEpcQuantityList(field, EpcType.InputQuantity)); break;
                    case "outputQuantityList":
                        Event.Epcs.AddRange(ParseEpcQuantityList(field, EpcType.OutputQuantity)); break;
                    case "bizTransactionList":
                        Event.Transactions.AddRange(ParseTransactionList(field)); break;
                    case "sourceList":
                        Event.Sources.AddRange(ParseSources(field)); break;
                    case "destinationList":
                        Event.Destinations.AddRange(ParseDestinations(field)); break;
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
                        Event.Epcs.AddRange(ParseEpcList(field, EpcType.ChildEpc)); break;
                    case "inputEPCList":
                        Event.Epcs.AddRange(ParseEpcList(field, EpcType.InputEpc)); break;
                    case "outputEPCList":
                        Event.Epcs.AddRange(ParseEpcList(field, EpcType.OutputEpc)); break;
                    case "quantityList":
                        Event.Epcs.AddRange(ParseEpcQuantityList(field, EpcType.Quantity)); break;
                    case "childQuantityList":
                        Event.Epcs.AddRange(ParseEpcQuantityList(field, EpcType.ChildQuantity)); break;
                    case "inputQuantityList":
                        Event.Epcs.AddRange(ParseEpcQuantityList(field, EpcType.InputQuantity)); break;
                    case "outputQuantityList":
                        Event.Epcs.AddRange(ParseEpcQuantityList(field, EpcType.OutputQuantity)); break;
                    case "bizTransactionList":
                        Event.Transactions.AddRange(ParseTransactionList(field)); break;
                    case "sourceList":
                        Event.Sources.AddRange(ParseSources(field)); break;
                    case "destinationList":
                        Event.Destinations.AddRange(ParseDestinations(field)); break;
                    case "ilmd":
                        ParseFields(field, FieldType.Ilmd); break;
                    case "persistentDisposition":
                        Event.PersistentDispositions.AddRange(ParsePersistentDisposition(field)); break;
                    case "sensorElementList":
                        Event.SensorElements.AddRange(ParseSensorList(field)); break;
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
                        Event.EventId = field.Value; break;
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
}
