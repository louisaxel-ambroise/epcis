using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;

namespace FasTnT.Features.v2_0.Communication.Xml.Parsers;

public static class XmlEventParser
{
    public static IEnumerable<Event> ParseEvents(XElement root)
    {
        return root.Elements().Select(ParseEvent);
    }

    private static Event ParseEvent(XElement element)
    {
        var evt = new Event
        {
            Type = Enum.Parse<EventType>(element.Name.LocalName)
        };

        foreach (var field in element.Elements())
        {
            if (string.IsNullOrEmpty(field.Name.NamespaceName))
            {
                switch (field.Name.LocalName)
                {
                    case "action":
                        evt.Action = Enum.Parse<EventAction>(field.Value, true); break;
                    case "eventTime":
                        evt.EventTime = DateTime.Parse(field.Value); break;
                    case "eventTimeZoneOffset":
                        evt.EventTimeZoneOffset = field.Value; break;
                    case "bizStep":
                        evt.BusinessStep = field.Value; break;
                    case "disposition":
                        evt.Disposition = field.Value; break;
                    case "transformationID":
                        evt.TransformationId = field.Value; break;
                    case "readPoint":
                        evt.ReadPoint = field.Element("id").Value; break;
                    case "bizLocation":
                        evt.BusinessLocation = field.Element("id").Value; break;
                    case "eventID":
                        evt.EventId = field.Value; break;
                    case "parentID":
                        evt.Epcs.Add(new Epc { Type = EpcType.ParentId, Id = field.Value }); break;
                    case "epcList":
                        evt.Epcs.AddRange(ParseEpcList(field, EpcType.List)); break;
                    case "childEPCs":
                        evt.Epcs.AddRange(ParseEpcList(field, EpcType.ChildEpc)); break;
                    case "inputEPCList":
                        evt.Epcs.AddRange(ParseEpcList(field, EpcType.InputEpc)); break;
                    case "outputEPCList":
                        evt.Epcs.AddRange(ParseEpcList(field, EpcType.OutputEpc)); break;
                    case "quantityList":
                        evt.Epcs.AddRange(ParseQuantityEpcList(field, EpcType.Quantity)); break;
                    case "childQuantityList":
                        evt.Epcs.AddRange(ParseQuantityEpcList(field, EpcType.ChildQuantity)); break;
                    case "inputQuantityList":
                        evt.Epcs.AddRange(ParseQuantityEpcList(field, EpcType.InputQuantity)); break;
                    case "outputQuantityList":
                        evt.Epcs.AddRange(ParseQuantityEpcList(field, EpcType.InputQuantity)); break;
                    case "bizTransactionList":
                        evt.Transactions.AddRange(ParseTransactionList(field)); break;
                    case "sourceList":
                        evt.Sources.AddRange(ParseSourceList(field)); break;
                    case "destinationList":
                        evt.Destinations.AddRange(ParseDestinationList(field)); break;
                    case "persistentDisposition":
                        evt.PersistentDispositions.AddRange(ParsePersustentDisposition(field)); break;
                    case "sensorElementList":
                        /* TODO: parse sensorElementList */ break;
                    case "ilmd":
                        /* TODO: parse ILMD */ break;
                    default:
                        throw new EpcisException(ExceptionType.ImplementationException, $"Unexpected event field: {field.Name}");
                }
            }
            else
            {
                // add to custom fields.
            }
        }

        return evt;
    }

    private static IEnumerable<Epc> ParseEpcList(XElement field, EpcType type)
    {
        return field.Elements().Select(x => new Epc
        {
            Id = x.Value,
            Type = type
        });
    }

    private static IEnumerable<Epc> ParseQuantityEpcList(XElement field, EpcType type)
    {
        return field.Elements().Select(x => new Epc
        {
            Id = x.Element("epcClass").Value,
            Quantity = float.Parse(x.Element("quantity")?.Value),
            UnitOfMeasure = x.Element("uom")?.Value,
            IsQuantity = true,
            Type = type,
        });
    }

    private static IEnumerable<BusinessTransaction> ParseTransactionList(XElement field)
    {
        return field.Elements().Select(x => new BusinessTransaction
        {
            Id = x.Value,
            Type = x.Attribute("type").Value
        });
    }

    private static IEnumerable<Source> ParseSourceList(XElement field)
    {
        return field.Elements().Select(x => new Source
        {
            Id = x.Value,
            Type = x.Attribute("type").Value
        });
    }

    private static IEnumerable<Destination> ParseDestinationList(XElement field)
    {
        return field.Elements().Select(x => new Destination
        {
            Id = x.Value,
            Type = x.Attribute("type").Value
        });
    }

    private static IEnumerable<PersistentDisposition> ParsePersustentDisposition(XElement field)
    {
        return field.Elements().Select(x => new PersistentDisposition
        {
            Id = x.Value,
            Type = Enum.Parse<PersistentDispositionType>(x.Name.LocalName, true)
        });
    }
}
