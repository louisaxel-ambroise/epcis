using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Events;

namespace FasTnT.Host.Communication.Xml.Parsers;

public class XmlV2EventParser : XmlEventParser
{
    public override Event ParseEvent(XElement element)
    {
        Index = 0;
        Event = new Event
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
                        Event.Action = Enum.Parse<EventAction>(field.Value, true); break;
                    case "recordTime": // Discard - this will be overridden
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
                        Event.Epcs.AddRange(ParseQuantityEpcList(field, EpcType.Quantity)); break;
                    case "childQuantityList":
                        Event.Epcs.AddRange(ParseQuantityEpcList(field, EpcType.ChildQuantity)); break;
                    case "inputQuantityList":
                        Event.Epcs.AddRange(ParseQuantityEpcList(field, EpcType.InputQuantity)); break;
                    case "outputQuantityList":
                        Event.Epcs.AddRange(ParseQuantityEpcList(field, EpcType.OutputQuantity)); break;
                    case "bizTransactionList":
                        Event.Transactions.AddRange(ParseTransactionList(field)); break;
                    case "sourceList":
                        Event.Sources.AddRange(ParseSources(field)); break;
                    case "destinationList":
                        Event.Destinations.AddRange(ParseDestinations(field)); break;
                    case "persistentDisposition":
                        Event.PersistentDispositions.AddRange(ParsePersistentDisposition(field)); break;
                    case "sensorElementList":
                        Event.SensorElements.AddRange(ParseSensorList(field)); break;
                    case "ilmd":
                        ParseFields(field, FieldType.Ilmd); break;
                    default:
                        throw new EpcisException(ExceptionType.ImplementationException, $"Unexpected event field: {field.Name}");
                }
            }
            else
            {
                ParseCustomFields(field, FieldType.CustomField, null, null);
            }
        }

        return Event;
    }

    private static IEnumerable<Epc> ParseQuantityEpcList(XElement field, EpcType type)
    {
        return field.Elements().Select(x => new Epc
        {
            Id = x.Element("epcClass").Value,
            Quantity = float.TryParse(x.Element("quantity")?.Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-GB"), out float quantity) ? quantity : default(float?),
            UnitOfMeasure = x.Element("uom")?.Value,
            Type = type,
        });
    }
}
