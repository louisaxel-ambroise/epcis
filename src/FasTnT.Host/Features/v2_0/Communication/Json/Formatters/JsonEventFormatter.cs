using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model.Events;
using FasTnT.Host.Features.v2_0.Communication.Json.Utils;

namespace FasTnT.Host.Features.v2_0.Communication.Json.Formatters;

public class JsonEventFormatter
{
    private readonly Event _evt;
    private readonly IDictionary<string, string> _context;

    private JsonEventFormatter(Event evt, IDictionary<string, string> context)
    {
        _evt = evt;
        _context = context;
    }

    public static IDictionary<string, object> FormatEvent(Event evt, IDictionary<string, string> context)
    {
        var formatter = new JsonEventFormatter(evt, context);

        return formatter.FormatEvent();
    }

    internal IDictionary<string, object> FormatEvent()
    {
        var element = new Dictionary<string, object>
        {
            ["type"] = _evt.Type.ToString(),
            ["eventTime"] = _evt.EventTime,
            ["recordTime"] = _evt.CaptureTime,
            ["eventTimeZoneOffset"] = _evt.EventTimeZoneOffset.Representation,
            ["eventID"] = _evt.EventId
        };

        if (_evt.CorrectiveDeclarationTime.HasValue)
        {
            element["errorDeclaration"] = new Dictionary<string, object>
            {
                ["declarationTime"] = _evt.CorrectiveDeclarationTime.Value,
                ["reason"] = _evt.CorrectiveReason,
                ["correctiveEventIDs"] = _evt.CorrectiveEventIds.Select(x => x.CorrectiveId)
            };
        }
        if (_evt.Action != EventAction.None)
        {
            element["action"] = _evt.Action.ToUpperString();
        }
        if (_evt.Epcs.Count > 0)
        {
            AddEpcs(element, _evt.Epcs);
        }
        element.AddIfNotNull(_evt.CertificationInfo, "certificationInfo");
        element.AddIfNotNull(_evt.TransformationId, "transformationID");
        element.AddIfNotNull(_evt.BusinessStep, "bizStep");
        element.AddIfNotNull(_evt.Disposition, "disposition");

        if (_evt.ReadPoint is not null)
        {
            var readPoint = new Dictionary<string, object> { { "id", _evt.ReadPoint } };
            var readPointFields = BuildExtensionFields(_evt.Fields.Where(x => x.Type == FieldType.ReadPointCustomField));

            foreach (var field in readPointFields)
            {
                readPoint[field.Key] = field.Value;
            }

            element["readPoint"] = readPoint;
        }
        if (_evt.BusinessLocation is not null)
        {
            var bizLocation = new Dictionary<string, object> { { "id", _evt.ReadPoint } };
            var bizLocationFields = BuildExtensionFields(_evt.Fields.Where(x => x.Type == FieldType.ReadPointCustomField));

            foreach (var field in bizLocationFields)
            {
                bizLocation[field.Key] = field.Value;
            }

            element["bizLocation"] = bizLocation;
        }
        if (_evt.Sources.Count > 0)
        {
            element["sourceList"] = _evt.Sources.Select(x => new { type = x.Type, source = x.Id });
        }
        if (_evt.Destinations.Count > 0)
        {
            element["destinationList"] = _evt.Destinations.Select(x => new { type = x.Type, destination = x.Id });
        }
        if (_evt.Transactions.Count > 0)
        {
            element["bizTransactionList"] = _evt.Transactions.Select(x => new { type = string.IsNullOrEmpty(x.Type) ? null : x.Type, bizTransaction = x.Id });
        }
        if (_evt.PersistentDispositions.Count > 0)
        {
            SetDisposition(element, _evt.PersistentDispositions);
        }

        AddSensorElements(element, _evt.SensorElements);

        var ilmd = BuildExtensionFields(_evt.Fields.Where(x => x.Type == FieldType.Ilmd));
        if (ilmd.Count > 0)
        {
            element["ilmd"] = ilmd;
        }

        var customFields = BuildExtensionFields(_evt.Fields.Where(x => x.Type == FieldType.CustomField));
        foreach (var field in customFields)
        {
            element[field.Key] = field.Value;
        }

        return element;
    }

    private void AddSensorElements(Dictionary<string, object> element, List<SensorElement> sensorElements)
    {
        if (sensorElements.Count == 0)
        {
            return;
        }

        element["sensorElements"] = sensorElements.Select(MapSensorElement);
    }

    private object MapSensorElement(SensorElement sensor)
    {
        var element = new Dictionary<string, object>
        {
            ["type"] = "epcis:SensorElement",
            ["sensorMetadata"] = MapSensorMetadata(sensor),
            ["sensorReport"] = _evt.Reports.Where(x => x.SensorIndex == sensor.Index).Select(MapSensorReport)
        };

        var customFields = BuildExtensionFields(_evt.Fields.Where(x => x.Type == FieldType.Sensor && x.ParentIndex == sensor.Index));
        foreach (var field in customFields)
        {
            element[field.Key] = field.Value;
        }

        return element;
    }

    private object MapSensorMetadata(SensorElement sensor)
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

        var customFields = BuildExtensionFields(_evt.Fields.Where(x => x.Type == FieldType.SensorMetadata && x.EntityIndex == sensor.Index));
        foreach (var field in customFields)
        {
            element[field.Key] = field.Value;
        }

        return element;
    }

    private object MapSensorReport(SensorReport report)
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
            ["deviceMetadata"] = report.DeviceMetadata,
            ["coordinateReferenceSystem"] = report.CoordinateReferenceSystem
        };

        var customFields = BuildExtensionFields(_evt.Fields.Where(x => x.Type == FieldType.SensorReport && x.EntityIndex == report.Index));
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

    private IDictionary<string, object> BuildExtensionFields(IEnumerable<Field> fields)
    {
        var extension = new Dictionary<string, object>();

        foreach (var group in fields.Where(x => x.ParentIndex == null).GroupBy(x => (x.Name, x.Namespace)))
        {
            if (group.Count() > 1)
            {
                extension.Add(_context[group.Key.Namespace] + ":" + group.Key.Name, BuildArrayElement(group));
            }
            else
            {
                var field = group.Single();

                if (fields.Any(x => x.Type != FieldType.Attribute && x.ParentIndex == field.Index))
                {
                    extension.Add(_context[field.Namespace] + ":" + field.Name, BuildElement(fields, field.Index));
                }
                else
                {
                    extension.Add(_context[field.Namespace] + ":" + field.Name, field.TextValue);
                }
            }
        }

        return extension;
    }

    private Dictionary<string, object> BuildElement(IEnumerable<Field> fields, int? parentIndex = null)
    {
        var element = new Dictionary<string, object>();

        foreach (var group in fields.Where(x => x.ParentIndex == parentIndex && x.Type != FieldType.Attribute).GroupBy(x => (x.Name, x.Namespace)))
        {
            if (group.Count() > 1)
            {
                var name = string.IsNullOrEmpty(group.Key.Namespace) ? group.Key.Name : _context[group.Key.Namespace] + ":" + group.Key.Name;

                element.Add(name, BuildArrayElement(group));
            }
            else
            {
                var field = group.Single();
                var name = string.IsNullOrEmpty(field.Namespace) ? field.Name : _context[field.Namespace] + ":" + field.Name;

                if (fields.Any(x => x.Type != FieldType.Attribute && x.ParentIndex == field.Index))
                {
                    element.Add(name, BuildElement(fields, field.Index));
                }
                else
                {
                    element.Add(name, field.TextValue);
                }
            }
        }

        return element;
    }

    private List<object> BuildArrayElement(IEnumerable<Field> fields)
    {
        var array = new List<object>();

        foreach (var field in fields)
        {
            var children = fields.Where(x => x.Type != FieldType.Attribute && x.ParentIndex == field.Index);

            if (children.Count() > 1 && children.All(x => x.Name == field.Name && x.Namespace == field.Namespace))
            {
                array.Add(BuildArrayElement(children));
            }
            else if (children.Any())
            {
                array.Add(BuildElement(children));
            }
            else
            {
                array.Add(field.TextValue);
            }
        }

        return array;
    }
}
