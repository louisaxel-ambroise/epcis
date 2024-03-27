using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model.Events;

namespace FasTnT.Host.Communication.Xml.Formatters;

public sealed class XmlV1EventFormatter : XmlEventFormatter
{
    public static XmlEventFormatter Instance { get; } = new XmlV1EventFormatter();

    public XmlV1EventFormatter()
    {
        Formatters = new()
        {
            { EventType.ObjectEvent, FormatObjectEvent },
            { EventType.QuantityEvent, FormatQuantityEvent },
            { EventType.AggregationEvent, FormatAggregationEvent },
            { EventType.TransactionEvent, FormatTransactionEvent },
            { EventType.TransformationEvent, FormatTransformationEvent },
            { EventType.AssociationEvent, FormatAssociationEvent },
        };
    }

    private static XElement FormatObjectEvent(Event evt)
    {
        var xmlEvent = new XElement("ObjectEvent");

        AddCommonEventFields(evt, xmlEvent);
        xmlEvent.Add(CreateEpcList(evt, EpcType.List, "epcList"));
        xmlEvent.Add(new XElement("action", evt.Action.ToString().ToUpper()));
        AddV1_1Fields(evt, xmlEvent);
        xmlEvent.AddIfNotNull(CreateBizTransactions(evt));
        xmlEvent.AddIfNotNull(FormatObjectEventExtension(evt));
        xmlEvent.AddIfNotNull(CreateCustomFields(evt, FieldType.CustomField));

        return xmlEvent;
    }

    private static XElement FormatObjectEventExtension(Event evt)
    {
        var extension = new XElement("extension");
        extension.AddIfNotNull(CreateQuantityList(evt, EpcType.Quantity, "quantityList"));
        extension.AddIfNotNull(CreateSourceList(evt));
        extension.AddIfNotNull(CreateDestinationList(evt));
        extension.AddIfNotNull(CreateFromCustomFields(evt, FieldType.Ilmd, "ilmd"));
        extension.AddIfNotNull(CreateV2Extension(evt));

        return extension;
    }

    private static XElement FormatQuantityEvent(Event evt)
    {
        var xmlEvent = new XElement("QuantityEvent");

        AddCommonEventFields(evt, xmlEvent);
        xmlEvent.Add(new XElement("epcClass", evt.Epcs.Single().Id));
        xmlEvent.Add(new XElement("quantity", evt.Epcs.Single().Quantity));
        AddV1_1Fields(evt, xmlEvent);
        xmlEvent.AddIfNotNull(CreateBizTransactions(evt));
        xmlEvent.AddIfNotNull(CreateFromCustomFields(evt, FieldType.Extension, "extension"));
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
        xmlEvent.AddIfNotNull(FormatAggregationEventExtension(evt));
        xmlEvent.AddIfNotNull(CreateCustomFields(evt, FieldType.CustomField));

        return xmlEvent;
    }

    private static XElement FormatAggregationEventExtension(Event evt)
    {
        var extension = new XElement("extension");
        extension.AddIfNotNull(CreateQuantityList(evt, EpcType.Quantity, "childQuantityList"));
        extension.AddIfNotNull(CreateSourceList(evt));
        extension.AddIfNotNull(CreateDestinationList(evt));
        extension.AddIfNotNull(CreateV2Extension(evt));

        return extension;
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
        xmlEvent.AddIfNotNull(FormatTransactionEventExtension(evt));
        xmlEvent.AddIfNotNull(CreateCustomFields(evt, FieldType.CustomField));

        return xmlEvent;
    }

    private static XElement FormatTransactionEventExtension(Event evt)
    {
        var extension = new XElement("extension");
        extension.AddIfNotNull(CreateQuantityList(evt, EpcType.Quantity, "quantityList"));
        extension.AddIfNotNull(CreateSourceList(evt));
        extension.AddIfNotNull(CreateDestinationList(evt));
        extension.AddIfNotNull(CreateV2Extension(evt));

        return extension;
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
        xmlEvent.AddIfNotNull(CreateFromCustomFields(evt, FieldType.Ilmd, "ilmd"));
        xmlEvent.AddIfNotNull(CreateV2Extension(evt));
        xmlEvent.AddIfNotNull(CreateCustomFields(evt, FieldType.CustomField));

        return new XElement("extension", xmlEvent);
    }

    private static XElement FormatAssociationEvent(Event evt)
    {
        var xmlEvent = new XElement("AssociationEvent");

        AddCommonEventFields(evt, xmlEvent);
        xmlEvent.AddIfNotNull(new XElement("parentID", evt.Epcs.FirstOrDefault(x => x.Type == EpcType.ParentId)?.Id));
        xmlEvent.Add(CreateEpcList(evt, EpcType.ChildEpc, "childEPCs"));
        xmlEvent.Add(CreateQuantityList(evt, EpcType.ChildQuantity, "childQuantityList"));
        xmlEvent.Add(new XElement("action", evt.Action.ToString().ToUpper()));
        AddV1_1Fields(evt, xmlEvent);
        xmlEvent.AddIfNotNull(CreateBizTransactions(evt));
        xmlEvent.AddIfNotNull(CreateSourceList(evt));
        xmlEvent.AddIfNotNull(CreateDestinationList(evt));
        xmlEvent.AddIfNotNull(CreateIlmdFields(evt));
        xmlEvent.AddIfNotNull(CreateSensorElementList(evt));
        xmlEvent.AddIfNotNull(CreatePersistentDispositionList(evt));
        xmlEvent.AddIfNotNull(CreateFromCustomFields(evt, FieldType.Extension, "extension"));
        xmlEvent.AddIfNotNull(CreateCustomFields(evt, FieldType.CustomField));

        return new XElement("extension", new XElement("extension", xmlEvent));
    }

    private static void AddCommonEventFields(Event evt, XElement xmlEvent)
    {
        xmlEvent.Add(new XElement("eventTime", evt.EventTime.ToString("yyyy-MM-ddTHH:mm:ssZ")));
        xmlEvent.Add(new XElement("recordTime", evt.Request.RecordTime.ToString("yyyy-MM-ddTHH:mm:ssZ")));
        xmlEvent.Add(new XElement("eventTimeZoneOffset", evt.EventTimeZoneOffset.Representation));
        xmlEvent.AddIfNotNull(CreateBaseExtension(evt));
    }

    private static XElement CreateBaseExtension(Event evt)
    {
        var baseExtension = new XElement("baseExtension");

        baseExtension.AddIfNotNull(new XElement("eventID", evt.EventId));
        baseExtension.AddIfNotNull(CreateErrorDeclaration(evt));
        baseExtension.AddIfNotNull(CreateCustomFields(evt, FieldType.BaseExtension));

        return baseExtension;
    }

    private static XElement CreateV2Extension(Event evt)
    {
        var extension = new XElement("extension");
        extension.AddIfNotNull(CreateSensorElementList(evt));
        extension.AddIfNotNull(CreatePersistentDispositionList(evt));
        extension.AddIfNotNull(CreateCustomFields(evt, FieldType.Extension));

        return extension;
    }
}
