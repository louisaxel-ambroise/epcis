using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model.Events;

namespace FasTnT.Host.Communication.Xml.Formatters;

public sealed class XmlV2EventFormatter : XmlEventFormatter
{
    public static XmlEventFormatter Instance { get; } = new XmlV2EventFormatter();

    public XmlV2EventFormatter()
    {
        Formatters = new()
        {
            { EventType.ObjectEvent, FormatObjectEvent },
            { EventType.AggregationEvent, FormatAggregationEvent },
            { EventType.TransactionEvent, FormatTransactionEvent },
            { EventType.TransformationEvent, FormatTransformationEvent },
            { EventType.AssociationEvent, FormatAssociationEvent }
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

    private static void AddCommonEventFields(Event evt, XElement xmlEvent)
    {
        xmlEvent.Add(new XElement("eventTime", evt.EventTime.ToString("yyyy-MM-ddTHH:mm:ssZ")));
        xmlEvent.Add(new XElement("recordTime", evt.Request.RecordTime.ToString("yyyy-MM-ddTHH:mm:ssZ")));
        xmlEvent.Add(new XElement("eventTimeZoneOffset", evt.EventTimeZoneOffset.Representation));
        xmlEvent.AddIfNotNull(new XElement("eventID", evt.EventId));
        xmlEvent.AddIfNotNull(CreateErrorDeclaration(evt));
        xmlEvent.AddIfNotNull(new XElement("certificationInfo", evt.CertificationInfo));
        xmlEvent.AddIfNotNull(CreateCustomFields(evt, FieldType.BaseExtension));
    }
}