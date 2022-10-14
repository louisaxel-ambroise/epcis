using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model.Events;
using FasTnT.Features.v2_0.Communication.Json.Utils;

namespace FasTnT.Features.v2_0.Communication.Json.Formatters;

public static class JsonEventFormatter
{
    public static IDictionary<string, object> FormatEvent(Event evt, IDictionary<string, string> context)
    {
        var element = new Dictionary<string, object>
        {
            ["isA"] = evt.Type.ToString(),
            ["eventTime"] = evt.EventTime,
            ["recordTime"] = evt.CaptureTime,
            ["eventTimeZoneOffset"] = evt.EventTimeZoneOffset.Representation,
            ["eventID"] = evt.EventId
        };

        if (evt.Action != EventAction.None)
        {
            element["action"] = evt.Action.ToString();
        }
        if (evt.Epcs.Count > 0)
        {
            AddEpcs(element, evt.Epcs);
        }

        element.AddIfNotNull(evt.TransformationId, "transformationID");
        element.AddIfNotNull(evt.BusinessStep, "bizStep");
        element.AddIfNotNull(evt.Disposition, "disposition");

        if (evt.ReadPoint is not null)
        {
            element["readPoint"] = new { id = evt.ReadPoint };
        }
        if (evt.BusinessLocation is not null)
        {
            element["bizLocation"] = new { id = evt.BusinessLocation };
        }
        if (evt.Sources.Count > 0)
        {
            element["sourceList"] = evt.Sources.Select(x => new { type = x.Type, source = x.Id });
        }
        if (evt.Destinations.Count > 0)
        {
            element["destList"] = evt.Destinations.Select(x => new { type = x.Type, destination = x.Id });
        }
        if (evt.Transactions.Count > 0)
        {
            element["bizTransactionList"] = evt.Transactions.Select(x => new { type = x.Type, bizTransaction = x.Id });
        }
        if (evt.PersistentDispositions.Count > 0)
        {
            SetDisposition(element, evt.PersistentDispositions);
        }

        AddSensorElements(element, evt.SensorElements, context);

        var ilmd = BuildExtensionFields(evt.Fields.OfType<Field>().Where(x => x.Type == FieldType.Ilmd), context);
        if (ilmd.Count > 0)
        {
            element["ilmd"] = ilmd;
        }

        var customFields = BuildExtensionFields(evt.Fields.OfType<Field>().Where(x => x.Type == FieldType.CustomField), context);
        foreach (var field in customFields)
        {
            element[field.Key] = field.Value;
        }

        return element;
    }

    private static void AddSensorElements(Dictionary<string, object> element, List<SensorElement> sensorElements, IDictionary<string, string> context)
    {
        if (sensorElements.Count == 0)
        {
            return;
        }

        element["sensorElements"] = sensorElements.Select(x => MapSensorElement(x, context));
    }

    private static object MapSensorElement(SensorElement sensor, IDictionary<string, string> context)
    {
        var element = new Dictionary<string, object>
        {
            ["isA"] = "epcis:SensorElement",
            ["sensorMetadata"] = MapSensorMetadata(sensor, context),
            ["sensorReport"] = sensor.Reports.Select(x => MapSensorReport(x, context))
        };

        var customFields = BuildExtensionFields(sensor.Fields.Where(x => x.Type == FieldType.CustomField), context);
        foreach (var field in customFields)
        {
            element[field.Key] = field.Value;
        }

        return element;
    }

    private static object MapSensorMetadata(SensorElement sensor, IDictionary<string, string> context)
    {
        var element = new Dictionary<string, object>
        {
            ["time"] = sensor.Time,
            ["deviceID"] = sensor.DeviceId,
            ["deviceMetadata"] = sensor.DeviceMetadata,
            ["rawData"] = sensor.Time,
            ["startTime"] = sensor.Time,
            ["endTime"] = sensor.Time,
            ["dataProcessingMethod"] = sensor.Time,
            ["bizRules"] = sensor.Time
        };

        var customFields = BuildExtensionFields(sensor.Fields.Where(x => x.Type == FieldType.SensorMetadata), context);
        foreach (var field in customFields)
        {
            element[field.Key] = field.Value;
        }

        return element;
    }

    private static object MapSensorReport(SensorReport report, IDictionary<string, string> context)
    {
        var element = new Dictionary<string, object>
        {
            ["type"] = report.Type,
            ["deviceID"] = report.DeviceId,
            ["rawData"] = report.RawData,
            ["dataProcessingMethod"] = report.DataProcessingMethod,
            ["time"] = report.Time,
            ["microorganism"] = report.Microorganism,
            ["chemicalSubstance"] = report.ChemicalSubstance,
            ["value"] = report.Value,
            ["component"] = report.Component,
            ["stringValue"] = report.StringValue,
            ["booleanValue"] = report.BooleanValue,
            ["hexBinaryValue"] = report.HexBinaryValue,
            ["uriValue"] = report.UriValue,
            ["minValue"] = report.MinValue,
            ["maxValue"] = report.MaxValue,
            ["meanValue"] = report.MeanValue,
            ["percRank"] = report.PercRank,
            ["percValue"] = report.PercValue,
            ["uom"] = report.UnitOfMeasure,
            ["sDev"] = report.SDev,
            ["deviceMetadata"] = report.DeviceMetadata
        };

        var customFields = BuildExtensionFields(report.Fields, context);
        foreach (var field in customFields)
        {
            element[field.Key] = field.Value;
        }

        return element;
    }

    private static void AddEpcs(Dictionary<string, object> element, List<Epc> epcs)
    {
        element.AddIfNotNull(epcs.SingleOrDefault(x => x.Type == EpcType.ParentId)?.Id, "parentID");

        AddEpcList(element, "epcList", epcs.Where(x => x.Type == EpcType.List));
        AddEpcList(element, "childEPCs", epcs.Where(x => x.Type == EpcType.ChildEpc));
        AddEpcList(element, "inputEPCList", epcs.Where(x => x.Type == EpcType.InputEpc));
        AddQuantityEpcList(element, "inputQuantityList", epcs.Where(x => x.Type == EpcType.InputQuantity));
        AddEpcList(element, "outputEPCList", epcs.Where(x => x.Type == EpcType.OutputEpc));
        AddQuantityEpcList(element, "outputQuantityList", epcs.Where(x => x.Type == EpcType.OutputQuantity));
        AddQuantityEpcList(element, "childQuantityList", epcs.Where(x => x.Type == EpcType.ChildQuantity));
    }

    private static void AddEpcList(Dictionary<string, object> element, string key, IEnumerable<Epc> epcs)
    {
        if (!epcs.Any())
        {
            return;
        }

        element[key] = epcs.Select(x => x.Id);
    }

    private static void AddQuantityEpcList(Dictionary<string, object> element, string key, IEnumerable<Epc> epcs)
    {
        if (!epcs.Any())
        {
            return;
        }

        element[key] = epcs.Select(x => new { epcClass = x.Id, quantity = x.Quantity, uom = x.UnitOfMeasure });
    }

    private static void SetDisposition(Dictionary<string, object> element, IEnumerable<PersistentDisposition> disposition)
    {
        var set = disposition.Where(x => x.Type == PersistentDispositionType.Set).Select(x => x.Id);
        var unset = disposition.Where(x => x.Type == PersistentDispositionType.Unset).Select(x => x.Id);

        var dispositions = new Dictionary<string, IEnumerable<string>>(2);

        if (set.Any())
        {
            dispositions["set"] = set;
        }
        if (unset.Any())
        {
            dispositions["unset"] = unset;
        }

        element["persistentDisposition"] = dispositions;
    }

    private static IDictionary<string, object> BuildExtensionFields(IEnumerable<Field> fields, IDictionary<string, string> context)
    {
        var extension = new Dictionary<string, object>();

        foreach (var group in fields.Where(x => !x.HasParent).GroupBy(x => (x.Name, x.Namespace)))
        {
            if (group.Count() > 1)
            {
                extension.Add(context[group.Key.Namespace] + ":" + group.Key.Name, BuildArrayElement(group, context));
            }
            else
            {
                var field = group.Single();
                var children = field.Children.Where(x => x.Type != FieldType.Attribute);

                if (children.Count() > 1)
                {
                    extension.Add(context[field.Namespace] + ":" + field.Name, BuildElement(children, context));
                }
                else if (children.Count() == 1)
                {
                    extension.Add(context[field.Namespace] + ":" + field.Name, BuildElement(children, context).First().Value);
                }
                else
                {
                    extension.Add(context[field.Namespace] + ":" + field.Name, field.TextValue);
                }
            }
        }

        return extension;
    }

    private static Dictionary<string, object> BuildElement(IEnumerable<Field> fields, IDictionary<string, string> context)
    {
        var element = new Dictionary<string, object>();

        foreach (var group in fields.GroupBy(x => (x.Name, x.Namespace)))
        {
            if (group.Count() > 1)
            {
                element.Add(context[group.Key.Namespace] + ":" + group.Key.Name, BuildArrayElement(group, context));
            }
            else
            {
                var field = group.Single();
                var children = field.Children.Where(x => x.Type != FieldType.Attribute);

                if (children.Any())
                {
                    element.Add(context[field.Namespace] + ":" + field.Name, BuildElement(children, context));
                }
                else
                {
                    element.Add(context[field.Namespace] + ":" + field.Name, field.TextValue);
                }
            }
        }

        return element;
    }

    private static List<object> BuildArrayElement(IEnumerable<Field> fields, IDictionary<string, string> context)
    {
        var array = new List<object>();

        foreach (var field in fields)
        {
            var children = field.Children.Where(x => x.Type != FieldType.Attribute);

            if (children.Count() > 1 && field.Children.All(x => x.Name == field.Name && x.Namespace == field.Namespace))
            {
                array.Add(BuildArrayElement(children, context));
            }
            else if (children.Any())
            {
                array.Add(BuildElement(children, context));
            }
            else
            {
                array.Add(field.TextValue);
            }
        }

        return array;
    }
}
