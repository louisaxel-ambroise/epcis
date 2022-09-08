using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model;
using FasTnT.Formatter.v2_0.Utils;

namespace FasTnT.Formatter.v2_0.Json.Formatters;
    
public static class JsonEventFormatter
{
    public static IDictionary<string, object> FormatEvent(Event evt)
    {
        var element = new Dictionary<string, object>
        {
            ["isA"] = evt.Type.ToString(),
            ["eventTime"] = evt.EventTime,
            ["recordTime"] = evt.CaptureTime,
            ["eventTimeZoneOffset"] = evt.EventTimeZoneOffset.Representation
        };

        if(evt.Action != EventAction.None)
        {
            element["action"] = evt.Action.ToString();
        }
        if(evt.Epcs.Count > 0)
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
        if(evt.Sources.Count > 0)
        {
            element["sourceList"] = evt.Sources.Select(x => new { type = x.Type, source = x.Id });
        }
        if(evt.Destinations.Count > 0)
        {
            element["destList"] = evt.Destinations.Select(x => new { type = x.Type, destination = x.Id });
        }
        if(evt.Transactions.Count > 0)
        {
            element["bizTransactionList"] = evt.Transactions.Select(x => new { type = x.Type, bizTransaction = x.Id });
        }
        if(evt.PersistentDispositions.Count > 0)
        {
            SetDisposition(element, evt.PersistentDispositions);
        }

        // TODO: add sensorElements
        // TODO: add context object and namespaces in custom fields

        var ilmd = BuildExtensionFields(evt.CustomFields.Where(x => x.Type == FieldType.Ilmd));
        if (ilmd is not null) element["ilmd"] = ilmd;

        var customFields = BuildExtensionFields(evt.CustomFields.Where(x => x.Type == FieldType.CustomField));
        if (customFields is not null)
        {
            foreach (var field in customFields) element[field.Key] = field.Value;
        }

        return element;
    }

    private static void AddEpcs(Dictionary<string, object> element, List<Epc> epcs)
    {
        element.AddIfNotNull(epcs.SingleOrDefault(x => x.Type == EpcType.ParentId)?.Id, "parentID");

        AddEpcList(element, "listEPC", epcs.Where(x => x.Type == EpcType.List));
        AddEpcList(element, "childEPC", epcs.Where(x => x.Type == EpcType.ChildEpc));
        AddEpcList(element, "inputEPCList", epcs.Where(x => x.Type == EpcType.InputEpc));
        AddQuantityEpcList(element, "inputQuantityList", epcs.Where(x => x.Type == EpcType.InputQuantity));
        AddEpcList(element, "outputEPCList", epcs.Where(x => x.Type == EpcType.OutputEpc));
        AddQuantityEpcList(element, "outputQuantityList", epcs.Where(x => x.Type == EpcType.OutputQuantity));
    }

    private static void AddEpcList(Dictionary<string, object> element, string key, IEnumerable<Epc> epcs)
    {
        if (!epcs.Any()) return;

        element[key] = epcs.Select(x => x.Id);
    }

    private static void AddQuantityEpcList(Dictionary<string, object> element, string key, IEnumerable<Epc> epcs)
    {
        if (!epcs.Any()) return;

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

    // TODO: refactor this mess.
    private static IDictionary<string, object> BuildExtensionFields(IEnumerable<CustomField> fields)
    {
        if (!fields.Any()) return null;

        var extension = new Dictionary<string, object>();

        foreach (var field in fields.Where(x => !x.HasParent))
        {
            if (field.Children.Count > 1)
            {
                if (field.Children.All(x => x.Name == field.Name && x.Namespace == field.Namespace))
                {
                    extension.Add(field.Namespace + ":" + field.Name, BuildArrayElement(field.Children));
                }
                else
                {
                    extension.Add(field.Namespace + ":" + field.Name, BuildElement(field.Children));
                }
            }
            else if (field.Children.Count == 1)
            {
                extension.Add(field.Namespace + ":" + field.Name, BuildElement(field.Children).First().Value);
            }
            else
            {
                extension.Add(field.Namespace + ":" + field.Name, field.TextValue);
            }
        }

        return extension;
    }

    private static Dictionary<string, object> BuildElement(IEnumerable<CustomField> fields)
    {
        var element = new Dictionary<string, object>();

        foreach (var field in fields)
        {
            if (field.Children.Count > 1)
            {
                if (field.Children.Count > 1 && field.Children.All(x => x.Name == field.Name && x.Namespace == field.Namespace))
                {
                    element.Add(field.Namespace + ":" + field.Name, BuildArrayElement(field.Children));
                }
                else
                {
                    element.Add(field.Namespace + ":" + field.Name, BuildArrayElement(field.Children));
                }
            }
            else if (field.Children.Count == 1)
            {
                element.Add(field.Namespace + ":" + field.Name, BuildArrayElement(field.Children).First());
            }
            else
            {
                element.Add(field.Namespace + ":" + field.Name, field.TextValue);
            }
        }

        return element;
    }

    private static List<object> BuildArrayElement(IEnumerable<CustomField> fields)
    {
        var array = new List<object>();

        foreach (var field in fields)
        {
            if (field.Children.Count > 1)
            {
                if (field.Children.All(x => x.Name == field.Name && x.Namespace == field.Namespace))
                {
                    array.Add(BuildArrayElement(field.Children));
                }
                else
                {
                    array.Add(BuildElement(field.Children));
                }
            }
            else if (field.Children.Count == 1)
            {
                array.Add(BuildElement(field.Children));
            }
            else
            {
                array.Add(field.TextValue);
            }
        }

        return array;
    }
}
