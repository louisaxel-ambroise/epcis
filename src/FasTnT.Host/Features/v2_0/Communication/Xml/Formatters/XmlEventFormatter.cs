using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Events;

namespace FasTnT.Host.Features.v2_0.Communication.Xml.Formatters;

public static class XmlEventFormatter
{
    static readonly IDictionary<EventType, Func<Event, XElement>> Formatters = new Dictionary<EventType, Func<Event, XElement>>
    {
        { EventType.ObjectEvent, FormatObjectEvent },
        { EventType.AggregationEvent, FormatAggregationEvent },
        { EventType.TransactionEvent, FormatTransactionEvent },
        { EventType.TransformationEvent, FormatTransformationEvent },
        { EventType.AssociationEvent, FormatAssociationEvent }
    };

    public static IEnumerable<XElement> FormatList(IList<Event> eventList)
    {
        return eventList.Select(FormatEvent);
    }

    private static XElement FormatEvent(Event evt)
    {
        return Formatters.TryGetValue(evt.Type, out Func<Event, XElement> formatter)
                ? formatter(evt)
                : throw new EpcisException(ExceptionType.NoSuchNameException, $"Unknown event type to format {evt?.Type}");
    }

    private static XElement FormatObjectEvent(Event evt)
    {
        var xmlEvent = new XElement("ObjectEvent");

        AddCommonEventFields(evt, xmlEvent);
        xmlEvent.Add(CreateEpcList(evt, EpcType.List, "epcList"));
        xmlEvent.Add(new XElement("action", evt.Action.ToString().ToUpper()));
        AddV1_1Fields(evt, xmlEvent);
        xmlEvent.AddIfNotNull(CreateBizTransactions(evt));
        xmlEvent.AddIfNotNull(CreateQuantityList(evt, EpcType.Quantity, "quantityList"));
        xmlEvent.AddIfNotNull(CreateSourceList(evt));
        xmlEvent.AddIfNotNull(CreateDestinationList(evt));
        xmlEvent.AddIfNotNull(CreateSensorElementList(evt));
        xmlEvent.AddIfNotNull(CreatePersistentDispositionList(evt));
        xmlEvent.AddIfNotNull(CreateFromCustomFields(evt, FieldType.Ilmd, "ilmd"));
        xmlEvent.AddIfNotNull(CreateFromCustomFields(evt, FieldType.Extension, "extension"));
        xmlEvent.AddIfNotNull(CreateCustomFields(evt, FieldType.CustomField));

        return xmlEvent;
    }

    private static XElement FormatAssociationEvent(Event evt)
    {
        var xmlEvent = new XElement("AssociationEvent");

        AddCommonEventFields(evt, xmlEvent);
        xmlEvent.AddIfNotNull(new XElement("parentID", evt.Epcs.FirstOrDefault(x => x.Type == EpcType.ParentId)?.Id));
        xmlEvent.Add(CreateEpcList(evt, EpcType.ChildEpc, "childEPCs"));
        xmlEvent.AddIfNotNull(CreateQuantityList(evt, EpcType.Quantity, "childQuantityList"));
        xmlEvent.Add(new XElement("action", evt.Action.ToString().ToUpper()));
        AddV1_1Fields(evt, xmlEvent);
        xmlEvent.AddIfNotNull(CreateBizTransactions(evt));
        xmlEvent.AddIfNotNull(CreateSourceList(evt));
        xmlEvent.AddIfNotNull(CreateDestinationList(evt));
        xmlEvent.AddIfNotNull(CreateSensorElementList(evt));
        xmlEvent.AddIfNotNull(CreateFromCustomFields(evt, FieldType.Ilmd, "ilmd"));
        xmlEvent.AddIfNotNull(CreateCustomFields(evt, FieldType.CustomField));

        return xmlEvent;
    }

    private static void AddV1_1Fields(Event evt, XElement xmlEvent)
    {
        xmlEvent.AddIfNotNull(new XElement("bizStep", evt.BusinessStep));
        xmlEvent.AddIfNotNull(new XElement("disposition", evt.Disposition));
        xmlEvent.AddIfNotNull(CreateReadPoint(evt));
        xmlEvent.AddIfNotNull(CreateBusinessLocation(evt));
    }

    private static XElement FormatAggregationEvent(Event evt)
    {
        var xmlEvent = new XElement("AggregationEvent");

        AddCommonEventFields(evt, xmlEvent);
        xmlEvent.AddIfNotNull(new XElement("parentID", evt.Epcs.FirstOrDefault(x => x.Type == EpcType.ParentId)?.Id));
        xmlEvent.Add(CreateEpcList(evt, EpcType.ChildEpc, "childEPCs"));
        xmlEvent.Add(new XElement("action", evt.Action.ToString().ToUpper()));
        AddV1_1Fields(evt, xmlEvent);
        xmlEvent.AddIfNotNull(CreateBizTransactions(evt));
        xmlEvent.AddIfNotNull(CreateQuantityList(evt, EpcType.Quantity, "childQuantityList"));
        xmlEvent.AddIfNotNull(CreateSourceList(evt));
        xmlEvent.AddIfNotNull(CreateDestinationList(evt));
        xmlEvent.AddIfNotNull(CreateSensorElementList(evt));
        xmlEvent.AddIfNotNull(CreateFromCustomFields(evt, FieldType.Extension, "extension"));
        xmlEvent.AddIfNotNull(CreateCustomFields(evt, FieldType.CustomField));

        return xmlEvent;
    }

    private static XElement FormatTransactionEvent(Event evt)
    {
        var xmlEvent = new XElement("TransactionEvent");

        AddCommonEventFields(evt, xmlEvent);
        xmlEvent.AddIfNotNull(CreateBizTransactions(evt));
        xmlEvent.AddIfNotNull(new XElement("parentID", evt.Epcs.FirstOrDefault(x => x.Type == EpcType.ParentId)?.Id));
        xmlEvent.Add(CreateEpcList(evt, EpcType.List, "epcList"));
        xmlEvent.Add(new XElement("action", evt.Action.ToString().ToUpper()));
        AddV1_1Fields(evt, xmlEvent);
        xmlEvent.AddIfNotNull(CreateQuantityList(evt, EpcType.Quantity, "quantityList"));
        xmlEvent.AddIfNotNull(CreateSourceList(evt));
        xmlEvent.AddIfNotNull(CreateDestinationList(evt));
        xmlEvent.AddIfNotNull(CreateSensorElementList(evt));
        xmlEvent.AddIfNotNull(CreateFromCustomFields(evt, FieldType.Extension, "extension"));
        xmlEvent.AddIfNotNull(CreateCustomFields(evt, FieldType.CustomField));

        return xmlEvent;
    }

    private static XElement FormatTransformationEvent(Event evt)
    {
        var xmlEvent = new XElement("TransformationEvent");

        AddCommonEventFields(evt, xmlEvent);
        xmlEvent.AddIfNotNull(CreateEpcList(evt, EpcType.InputEpc, "inputEPCList"));
        xmlEvent.AddIfNotNull(CreateQuantityList(evt, EpcType.InputQuantity, "inputQuantityList"));
        xmlEvent.AddIfNotNull(CreateEpcList(evt, EpcType.OutputEpc, "outputEPCList"));
        xmlEvent.AddIfNotNull(CreateQuantityList(evt, EpcType.OutputQuantity, "outputQuantityList"));
        xmlEvent.AddIfNotNull(new XElement("transformationID", evt.TransformationId));
        AddV1_1Fields(evt, xmlEvent);
        xmlEvent.AddIfNotNull(CreateBizTransactions(evt));
        xmlEvent.AddIfNotNull(CreateSourceList(evt));
        xmlEvent.AddIfNotNull(CreateDestinationList(evt));
        xmlEvent.AddIfNotNull(CreateSensorElementList(evt));
        xmlEvent.AddIfNotNull(CreateFromCustomFields(evt, FieldType.Ilmd, "ilmd"));
        xmlEvent.AddIfNotNull(CreateFromCustomFields(evt, FieldType.Extension, "extension"));
        xmlEvent.AddIfNotNull(CreateCustomFields(evt, FieldType.CustomField));

        return xmlEvent;
    }

    private static XElement CreateReadPoint(Event evt)
    {
        var readPointElement = new XElement("readPoint");
        readPointElement.AddIfNotNull(new XElement("id", evt.ReadPoint));
        readPointElement.AddIfNotNull(CreateFromCustomFields(evt, FieldType.ReadPointExtension, "extension"));
        readPointElement.AddIfNotNull(CreateCustomFields(evt, FieldType.ReadPointCustomField));

        return readPointElement;
    }

    private static XElement CreateBusinessLocation(Event evt)
    {
        var locationElement = new XElement("bizLocation");
        locationElement.AddIfNotNull(new XElement("id", evt.BusinessLocation));
        locationElement.AddIfNotNull(CreateFromCustomFields(evt, FieldType.BusinessLocationExtension, "extension"));
        locationElement.AddIfNotNull(CreateCustomFields(evt, FieldType.BusinessLocationCustomField));

        return locationElement;
    }

    private static XElement CreateSourceList(Event evt)
    {
        return new XElement("sourceList", evt.Sources.Select(x => new XElement("source", new XAttribute("type", x.Type), x.Id)));
    }

    private static XElement CreateDestinationList(Event evt)
    {
        return new XElement("destinationList", evt.Destinations.Select(x => new XElement("destination", new XAttribute("type", x.Type), x.Id)));
    }

    private static XElement CreateSensorElementList(Event evt)
    {
        var sensorElements = evt.SensorElements.Select(CreateSensorElement);

        return new XElement("sensorElementList", sensorElements);
    }

    private static XElement CreateSensorElement(SensorElement element)
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

        foreach (var field in element.Event.Fields.Where(x => x.Type == FieldType.SensorMetadata && x.EntityIndex == element.Index))
        {
            metadata.AddIfNotNull(new XAttribute(XName.Get(field.Name, field.Namespace), field.TextValue));
        }

        xmlElement.Add(metadata);
        xmlElement.AddIfNotNull(element.Reports.Select(CreateSensorReport));
        xmlElement.AddIfNotNull(FormatFields(element.Event.Fields.Where(x => x.Type == FieldType.Sensor && x.EntityIndex == element.Index)));

        return xmlElement;
    }

    private static XElement CreateSensorReport(SensorReport report)
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

        foreach (var field in report.SensorElement.Event.Fields.Where(x => x.Type == FieldType.SensoReport && x.EntityIndex == report.Index))
        {
            xmlElement.AddIfNotNull(new XAttribute(XName.Get(field.Name, field.Namespace), field.TextValue));
        }

        return xmlElement;
    }

    private static XAttribute CreateAttribute(string name, object value)
    {
        return value != null
            ? new XAttribute(name, value)
            : null;
    }

    private static XElement CreatePersistentDispositionList(Event evt)
    {
        var xmlElement = new XElement("persistentDisposition");
        xmlElement.AddIfNotNull(evt.PersistentDispositions.Select(CreatePersistentDisposition));

        return xmlElement;
    }

    private static XElement CreatePersistentDisposition(PersistentDisposition disposition)
    {
        var name = disposition.Type == PersistentDispositionType.Set ? "set" : "unset";

        return new XElement(name, disposition.Id);
    }

    private static XElement CreateFromCustomFields(Event evt, FieldType type, string elementName)
    {
        var extension = new XElement(elementName);
        extension.AddIfNotNull(CreateCustomFields(evt, type));

        return extension;
    }

    private static XElement CreateBizTransactions(Event evt)
    {
        var list = new XElement("bizTransactionList");

        list.AddIfNotNull(evt.Transactions.Select(x => new XElement("bizTransaction", new XAttribute("type", x.Type), x.Id)));

        return list;
    }

    private static XElement CreateEpcList(Event evt, EpcType type, string elementName)
    {
        var epcs = evt.Epcs.Where(x => x.Type == type);
        var list = new XElement(elementName);

        list.AddIfNotNull(epcs.Select(x => new XElement("epc", x.Id)));

        return list;
    }

    private static XElement CreateQuantityList(Event evt, EpcType type, string elementName)
    {
        var epcs = evt.Epcs.Where(x => x.Type == type);
        var list = new XElement(elementName);

        list.AddIfNotNull(epcs.Select(x =>
        {
            var element = new XElement("quantityElement", new XElement("epcClass", x.Id));
            element.AddIfNotNull(new[] { new XElement("quantity", x.Quantity), new XElement("uom", x.UnitOfMeasure) });

            return element;
        }));

        return list;
    }

    private static void AddCommonEventFields(Event evt, XElement xmlEvent)
    {
        xmlEvent.Add(new XElement("eventTime", evt.EventTime.ToString("yyyy-MM-ddTHH:mm:ssZ")));
        xmlEvent.Add(new XElement("recordTime", evt.CaptureTime.ToString("yyyy-MM-ddTHH:mm:ssZ")));
        xmlEvent.Add(new XElement("eventTimeZoneOffset", evt.EventTimeZoneOffset.Representation));
        xmlEvent.AddIfNotNull(new XElement("eventID", evt.EventId));
        xmlEvent.AddIfNotNull(CreateErrorDeclaration(evt));
        xmlEvent.AddIfNotNull(new XElement("certificationInfo", evt.CertificationInfo));
        xmlEvent.AddIfNotNull(CreateCustomFields(evt, FieldType.BaseExtension));
    }

    private static XElement CreateErrorDeclaration(Event evt)
    {
        var errorDeclaration = new XElement("errorDeclaration");

        errorDeclaration.AddIfNotNull(new XElement("declarationTime", evt.CorrectiveDeclarationTime));
        errorDeclaration.AddIfNotNull(new XElement("reason", evt.CorrectiveReason));
        errorDeclaration.AddIfNotNull(new XElement("correctiveEventIDs", evt.CorrectiveEventIds.Select(x => new XElement("correctiveEventID", x))));

        return errorDeclaration;
    }

    private static IEnumerable<XElement> CreateCustomFields(Event evt, FieldType type)
    {
        return FormatFields(evt.Fields.Where(x => x.Type == type));
    }

    private static IEnumerable<XElement> FormatFields(IEnumerable<Field> fields, int? parentIndex = null)
    {
        return fields.Where(x => x.ParentIndex == parentIndex).Select(x =>
        {
            var element = new XElement(XName.Get(x.Name, x.Namespace ?? string.Empty), x.TextValue);

            foreach (var attribute in fields.Where(i => i.Type == FieldType.Attribute && i.ParentIndex == x.Index))
            {
                element.Add(new XAttribute(XName.Get(attribute.Name, attribute.Namespace), attribute.TextValue));
            }

            if(fields.Any(i => i.Type != FieldType.Attribute && i.ParentIndex == x.Index))
            {
                element.AddIfNotNull(FormatFields(fields, x.Index));
            }

            return element;
        });
    }
}
