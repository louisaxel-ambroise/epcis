using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Events;
using FasTnT.Host.Communication.Xml.Utils;

namespace FasTnT.Host.Communication.Xml.Formatters;

public abstract class XmlEventFormatter
{
    protected Dictionary<EventType, Func<Event, XElement>> Formatters { get; init; }

    public IEnumerable<XElement> FormatList(IList<Event> eventList)
    {
        return eventList.Select(FormatEvent);
    }

    protected XElement FormatEvent(Event evt)
    {
        return Formatters.TryGetValue(evt.Type, out Func<Event, XElement> formatter)
                ? formatter(evt)
                : throw new EpcisException(ExceptionType.NoSuchNameException, $"Unknown event type to format {evt?.Type}");
    }

    protected static XElement CreateReadPoint(Event evt)
    {
        var readPointElement = new XElement("readPoint");
        readPointElement.AddIfNotNull(new XElement("id", evt.ReadPoint));
        readPointElement.AddIfNotNull(CreateFromCustomFields(evt, FieldType.ReadPointExtension, "extension"));
        readPointElement.AddIfNotNull(CreateCustomFields(evt, FieldType.ReadPointCustomField));

        return readPointElement;
    }

    protected static XElement CreateBusinessLocation(Event evt)
    {
        var locationElement = new XElement("bizLocation");
        locationElement.AddIfNotNull(new XElement("id", evt.BusinessLocation));
        locationElement.AddIfNotNull(CreateFromCustomFields(evt, FieldType.BusinessLocationExtension, "extension"));
        locationElement.AddIfNotNull(CreateCustomFields(evt, FieldType.BusinessLocationCustomField));

        return locationElement;
    }

    protected static XElement CreateSourceList(Event evt)
    {
        return new XElement("sourceList", evt.Sources.Select(x => new XElement("source", new XAttribute("type", x.Type), x.Id)));
    }

    protected static XElement CreateDestinationList(Event evt)
    {
        return new XElement("destinationList", evt.Destinations.Select(x => new XElement("destination", new XAttribute("type", x.Type), x.Id)));
    }

    protected static XElement CreateIlmdFields(Event evt)
    {
        var ilmd = new XElement("ilmd");
        ilmd.AddIfNotNull(CreateCustomFields(evt, FieldType.Ilmd));
        ilmd.AddIfNotNull(CreateFromCustomFields(evt, FieldType.IlmdExtension, "extension"));

        return ilmd;
    }

    protected static XElement CreateSensorElementList(Event evt)
    {
        var sensorElements = evt.SensorElements.Select(x => CreateSensorElement(x, evt.Reports, evt.Fields));

        return new XElement("sensorElementList", sensorElements);
    }

    protected static XElement CreateSensorElement(SensorElement element, List<SensorReport> reports, List<Field> fields)
    {
        var xmlElement = new XElement("sensorElement");

        var metadata = new XElement("sensorMetadata");
        metadata.AddIfNotNull(CreateAttribute("time", element.Time));
        metadata.AddIfNotNull(CreateAttribute("deviceID", element.DeviceId));
        metadata.AddIfNotNull(CreateAttribute("deviceMetadata", element.DeviceMetadata));
        metadata.AddIfNotNull(CreateAttribute("rawData", element.RawData));
        metadata.AddIfNotNull(CreateAttribute("startTime", element.StartTime));
        metadata.AddIfNotNull(CreateAttribute("endTime", element.EndTime));
        metadata.AddIfNotNull(CreateAttribute("bizRules", element.BizRules));
        metadata.AddIfNotNull(CreateAttribute("dataProcessingMethod", element.DataProcessingMethod));

        foreach (var field in fields.Where(x => x.Type == FieldType.SensorMetadata && x.EntityIndex == element.Index))
        {
            metadata.AddIfNotNull(new XAttribute(XName.Get(field.Name, field.Namespace), field.TextValue));
        }

        xmlElement.Add(metadata);
        xmlElement.AddIfNotNull(reports.Where(x => x.SensorIndex == element.Index).Select(x => CreateSensorReport(x, fields)));
        xmlElement.AddIfNotNull(fields.Where(x => x.Type == FieldType.Sensor && x.EntityIndex == element.Index).Select(x => FormatField(x, fields)));

        return xmlElement;
    }

    protected static XElement CreateSensorReport(SensorReport report, List<Field> fields)
    {
        var xmlElement = new XElement("sensorReport");
        xmlElement.AddIfNotNull(CreateAttribute("value", report.Value));
        xmlElement.AddIfNotNull(CreateAttribute("type", report.Type));
        xmlElement.AddIfNotNull(CreateAttribute("component", report.Component));
        xmlElement.AddIfNotNull(CreateAttribute("stringValue", report.StringValue));
        xmlElement.AddIfNotNull(CreateAttribute("booleanValue", report.BooleanValue));
        xmlElement.AddIfNotNull(CreateAttribute("hexBinaryValue", report.HexBinaryValue));
        xmlElement.AddIfNotNull(CreateAttribute("uriValue", report.UriValue));
        xmlElement.AddIfNotNull(CreateAttribute("uom", report.UnitOfMeasure));
        xmlElement.AddIfNotNull(CreateAttribute("minValue", report.MinValue));
        xmlElement.AddIfNotNull(CreateAttribute("maxValue", report.MaxValue));
        xmlElement.AddIfNotNull(CreateAttribute("sDev", report.SDev));
        xmlElement.AddIfNotNull(CreateAttribute("chemicalSubstance", report.ChemicalSubstance));
        xmlElement.AddIfNotNull(CreateAttribute("microorganism", report.Microorganism));
        xmlElement.AddIfNotNull(CreateAttribute("deviceID", report.DeviceId));
        xmlElement.AddIfNotNull(CreateAttribute("deviceMetadata", report.DeviceMetadata));
        xmlElement.AddIfNotNull(CreateAttribute("rawData", report.RawData));
        xmlElement.AddIfNotNull(CreateAttribute("time", report.Time));
        xmlElement.AddIfNotNull(CreateAttribute("meanValue", report.MeanValue));
        xmlElement.AddIfNotNull(CreateAttribute("percRank", report.PercRank));
        xmlElement.AddIfNotNull(CreateAttribute("percValue", report.PercValue));
        xmlElement.AddIfNotNull(CreateAttribute("dataProcessingMethod", report.DataProcessingMethod));
        xmlElement.AddIfNotNull(CreateAttribute("coordinateReferenceSystem", report.CoordinateReferenceSystem));

        foreach (var field in fields.Where(x => x.Type == FieldType.SensorReport && x.EntityIndex == report.Index))
        {
            xmlElement.AddIfNotNull(new XAttribute(XName.Get(field.Name, field.Namespace), field.TextValue));
        }

        return xmlElement;
    }

    protected static XAttribute CreateAttribute(string name, object value)
    {
        return value != null
            ? new XAttribute(name, value)
            : null;
    }

    protected static XElement CreatePersistentDispositionList(Event evt)
    {
        var xmlElement = new XElement("persistentDisposition");
        xmlElement.AddIfNotNull(evt.PersistentDispositions.Select(CreatePersistentDisposition));

        return xmlElement;
    }

    protected static XElement CreatePersistentDisposition(PersistentDisposition disposition)
    {
        var name = disposition.Type == PersistentDispositionType.Set ? "set" : "unset";

        return new XElement(name, disposition.Id);
    }

    protected static XElement CreateFromCustomFields(Event evt, FieldType type, string elementName)
    {
        var extension = new XElement(elementName);
        extension.AddIfNotNull(CreateCustomFields(evt, type));

        return extension;
    }

    protected static XElement CreateBizTransactions(Event evt)
    {
        var list = new XElement("bizTransactionList");

        list.AddIfNotNull(evt.Transactions.Select(x =>
        {
            var txElement = new XElement("bizTransaction", x.Id);

            if (!string.IsNullOrEmpty(x.Type))
            {
                txElement.Add(new XAttribute("type", x.Type));
            }

            return txElement;
        }));

        return list;
    }

    protected static XElement CreateEpcList(Event evt, EpcType type, string elementName)
    {
        var epcs = evt.Epcs.Where(x => x.Type == type);
        var list = new XElement(elementName);

        list.AddIfNotNull(epcs.Select(x => new XElement("epc", x.Id)));

        return list;
    }

    protected static XElement CreateQuantityList(Event evt, EpcType type, string elementName)
    {
        var epcs = evt.Epcs.Where(x => x.Type == type);
        var list = new XElement(elementName);

        list.AddIfNotNull(epcs.Select(x =>
        {
            var element = new XElement("quantityElement", new XElement("epcClass", x.Id));
            element.AddIfNotNull([new XElement("quantity", x.Quantity), new XElement("uom", x.UnitOfMeasure)]);

            return element;
        }));

        return list;
    }

    protected static XElement CreateErrorDeclaration(Event evt)
    {
        var errorDeclaration = new XElement("errorDeclaration");

        errorDeclaration.AddIfNotNull(new XElement("declarationTime", evt.CorrectiveDeclarationTime));
        errorDeclaration.AddIfNotNull(new XElement("reason", evt.CorrectiveReason));
        errorDeclaration.AddIfNotNull(new XElement("correctiveEventIDs", evt.CorrectiveEventIds.Select(x => new XElement("correctiveEventID", x))));

        return errorDeclaration;
    }

    protected static IEnumerable<XElement> CreateCustomFields(Event evt, FieldType type)
    {
        return evt.Fields.Where(x => x.Type == type && x.ParentIndex is null).Select(x => FormatField(x, evt.Fields));
    }

    protected static XElement FormatField(Field field, IEnumerable<Field> fields)
    {
        var attributes = fields.Where(x => x.ParentIndex == field.Index && x.Type == FieldType.Attribute).Select(x => new XAttribute(XName.Get(x.Name, x.Namespace), x.TextValue));
        var element = new XElement(XName.Get(field.Name, field.Namespace ?? string.Empty), field.TextValue, attributes);

        element.AddIfNotNull(fields.Where(x => x.ParentIndex == field.Index && x.Type != FieldType.Attribute).Select(x => FormatField(x, fields)));

        return element;
    }
}
