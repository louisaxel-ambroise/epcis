using FasTnT.Application.Services.DataSources.Utils;
using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;
using LinqKit;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using FasTnT.Application.Database;
using FasTnT.Domain.Model.Masterdata;
using FasTnT.Application.Handlers.DataSources.Utils;

namespace FasTnT.Application.Handlers.DataSources.Contexts;

public class EventQueryContext
{
    private bool _ascending;
    private readonly List<Func<IQueryable<Event>, IQueryable<Event>>> _pagination = new();
    private readonly List<Func<IQueryable<Event>, IQueryable<Event>>> _filters = new();
    private readonly EpcisContext _context;

    public EventQueryContext(EpcisContext context, IEnumerable<QueryParameter> parameters)
    {
        _context = context;

        foreach (var parameter in parameters)
        {
            ParseParameter(parameter);
        }
    }

    private void ParseParameter(QueryParameter param)
    {
        switch (param.Name)
        {
            // Order Parameters
            case "orderBy":
                ParseOrderField(param); break;
            case "orderDirection":
                _ascending = param.AsString() == "ASC"; break;
            // Simple filters
            case "eventType":
                Filter(x => param.Values.Select(x => Enum.Parse<EventType>(x, true)).Contains(x.Type)); break;
            case "nextPageToken":
                _pagination.Add(x => x.Skip(param.AsInt())); break;
            case "eventCountLimit" or "perPage" or "maxEventCount":
                _pagination.Add(x => x.Take(param.AsInt())); break;
            case "GE_eventTime":
                Filter(x => x.EventTime >= param.AsDate()); break;
            case "LT_eventTime":
                Filter(x => x.EventTime < param.AsDate()); break;
            case "GE_recordTime":
                Filter(x => x.CaptureTime >= param.AsDate()); break;
            case "LT_recordTime":
                Filter(x => x.CaptureTime < param.AsDate()); break;
            case "EQ_action":
                Filter(x => param.Values.Select(x => Enum.Parse<EventAction>(x, true)).Contains(x.Action)); break;
            case "EQ_bizLocation":
                Filter(x => param.Values.Contains(x.BusinessLocation)); break;
            case "EQ_bizStep":
                Filter(x => param.Values.Contains(x.BusinessStep)); break;
            case "EQ_disposition":
                Filter(x => param.Values.Contains(x.Disposition)); break;
            case "EQ_eventID":
                Filter(x => param.Values.Contains(x.EventId)); break;
            case "EQ_transformationID":
                Filter(x => param.Values.Contains(x.TransformationId)); break;
            case "EQ_readPoint":
                Filter(x => param.Values.Contains(x.ReadPoint)); break;
            case "EQ_userID":
                Filter(x => param.Values.Contains(x.Request.UserId)); break;
            case "EXISTS_errorDeclaration":
                Filter(x => x.CorrectiveDeclarationTime.HasValue); break;
            case "EQ_errorReason":
                Filter(x => param.Values.Contains(x.CorrectiveReason)); break;
            case "EQ_correctiveEventID":
                Filter(x => x.CorrectiveEventIds.Any(ce => param.Values.Contains(ce.CorrectiveId))); break;
            case "WD_readPoint":
                Filter(x => _context.Set<ReadPointHierarchy>().Any(h => h.Root == x.ReadPoint && param.Values.Contains(h.Id))); break;
            case "WD_bizLocation":
                Filter(x => _context.Set<BizLocationHierarchy>().Any(h => h.Root == x.BusinessLocation && param.Values.Contains(h.Id))); break;
            case "EQ_requestID":
                Filter(x => param.Values.Select(int.Parse).Contains(x.Request.Id)); break;
            case "EQ_captureID":
                Filter(x => param.Values.Contains(x.Request.CaptureId)); break;
            case "EQ_quantity":
                Filter(x => x.Epcs.Any(e => e.Type == EpcType.Quantity && e.Quantity == param.AsFloat())); break;
            case "GT_quantity":
                Filter(x => x.Epcs.Any(e => e.Type == EpcType.Quantity && e.Quantity > param.AsFloat())); break;
            case "GE_quantity":
                Filter(x => x.Epcs.Any(e => e.Type == EpcType.Quantity && e.Quantity >= param.AsFloat())); break;
            case "LT_quantity":
                Filter(x => x.Epcs.Any(e => e.Type == EpcType.Quantity && e.Quantity < param.AsFloat())); break;
            case "LE_quantity":
                Filter(x => x.Epcs.Any(e => e.Type == EpcType.Quantity && e.Quantity <= param.AsFloat())); break;
            // parameters introduced in EPCIS 2.0
            case "GE_startTime":
                Filter(x => x.SensorElements.Any(s => s.StartTime >= param.AsDate())); break;
            case "LT_startTime":
                Filter(x => x.SensorElements.Any(s => s.StartTime < param.AsDate())); break;
            case "GE_endTime":
                Filter(x => x.SensorElements.Any(s => s.EndTime >= param.AsDate())); break;
            case "LT_endTime":
                Filter(x => x.SensorElements.Any(s => s.EndTime < param.AsDate())); break;
            case "EQ_type":
                Filter(x => x.SensorElements.Any(s => s.Reports.Any(r => r.Type == param.AsString()))); break;
            case "EQ_deviceID":
                Filter(x => x.SensorElements.Any(s => s.Reports.Any(r => r.DeviceId == param.AsString()))); break;
            case "EQ_dataProcessingMethod":
                Filter(x => x.SensorElements.Any(s => s.Reports.Any(r => param.Values.Contains(r.DataProcessingMethod)))); break;
            case "EQ_microorganism":
                Filter(x => x.SensorElements.Any(s => s.Reports.Any(r => param.Values.Contains(r.Microorganism)))); break;
            case "EQ_chemicalSubstance":
                Filter(x => x.SensorElements.Any(s => s.Reports.Any(r => param.Values.Contains(r.ChemicalSubstance)))); break;
            case "EQ_bizRules":
                Filter(x => x.SensorElements.Any(s => param.Values.Contains(s.BizRules))); break;
            case "EQ_stringValue":
                Filter(x => x.SensorElements.Any(e => e.Reports.Any(r => r.StringValue == param.AsString()))); break;
            case "EQ_booleanValue":
                Filter(x => x.SensorElements.Any(e => e.Reports.Any(r => r.BooleanValue == param.AsBool()))); break;
            case "EQ_hexBinaryValue":
                Filter(x => x.SensorElements.Any(e => e.Reports.Any(r => param.Values.Contains(r.HexBinaryValue)))); break;
            case "EQ_uriValue":
                Filter(x => x.SensorElements.Any(e => e.Reports.Any(r => param.Values.Contains(r.UriValue)))); break;
            case "GE_percRank":
                Filter(x => x.SensorElements.Any(s => s.Reports.Any(r => r.PercRank >= param.AsFloat()))); break;
            case "LT_percRank":
                Filter(x => x.SensorElements.Any(s => s.Reports.Any(r => r.PercRank < param.AsFloat()))); break;
            case "EQ_persistentDisposition_set":
                ApplyPersistenDispositionFilter(param, PersistentDispositionType.Set); break;
            case "EQ_persistentDisposition_unset":
                ApplyPersistenDispositionFilter(param, PersistentDispositionType.Unset); break;
            // Family filters
            case var s when s.StartsWith("MATCH_"):
                ApplyMatchParameter(param); break;
            case var s when s.StartsWith("EQ_source_"):
                Filter(x => x.Sources.Any(s => s.Id == param.GetSimpleId() && param.Values.Contains(s.Type))); break;
            case var s when s.StartsWith("EQ_destination_"):
                Filter(x => x.Destinations.Any(d => d.Id == param.GetSimpleId() && param.Values.Contains(d.Type))); break;
            case var s when s.StartsWith("EQ_bizTransaction_"):
                Filter(x => x.Transactions.Any(t => t.Id == param.GetSimpleId() && param.Values.Contains(t.Type))); break;
            case var s when s.StartsWith("EQ_INNER_ILMD_"):
                ApplyFieldParameter(param.Values, FieldType.Ilmd, true, param.InnerIlmdName(), param.InnerIlmdNamespace()); break;
            case var s when s.StartsWith("EQ_ILMD_"):
                ApplyFieldParameter(param.Values, FieldType.Ilmd, false, param.IlmdName(), param.IlmdNamespace()); break;
            case var s when s.StartsWith("EQ_INNER_SENSORELEMENT_"):
                ApplyFieldParameter(param.Values, FieldType.Sensor, true, param.InnerIlmdName(), param.InnerIlmdNamespace()); break;
            case var s when s.StartsWith("EQ_SENSORELEMENT_"):
                ApplyFieldParameter(param.Values, FieldType.Sensor, false, param.IlmdName(), param.IlmdNamespace()); break;
            case var s when s.StartsWith("EQ_SENSORMETADATA_"):
                ApplyFieldParameter(param.Values, FieldType.SensorMetadata, false, param.IlmdName(), param.IlmdNamespace()); break;
            case var s when s.StartsWith("EQ_INNER_SENSORMETADATA_"):
                ApplyFieldParameter(param.Values, FieldType.SensorMetadata, true, param.InnerIlmdName(), param.InnerIlmdNamespace()); break;
            case var s when s.StartsWith("EQ_SENSORREPORT_"):
                ApplyFieldParameter(param.Values, FieldType.SensorReport, false, param.IlmdName(), param.IlmdNamespace()); break;
            case var s when s.StartsWith("EQ_INNER_SENSORREPORT_"):
                ApplyFieldParameter(param.Values, FieldType.SensorReport, true, param.InnerIlmdName(), param.InnerIlmdNamespace()); break;
            case var s when s.StartsWith("EXISTS_INNER_ILMD_"):
                ApplyExistsFieldParameter(FieldType.Ilmd, true, param.InnerIlmdName(), param.InnerIlmdNamespace()); break;
            case var s when s.StartsWith("EXISTS_ILMD_"):
                ApplyExistsFieldParameter(FieldType.Ilmd, false, param.IlmdName(), param.IlmdNamespace()); break;
            case var s when s.StartsWith("EXISTS_INNER_"):
                ApplyExistsFieldParameter(FieldType.CustomField, true, param.InnerFieldName(), param.InnerFieldNamespace()); break;
            case var s when s.StartsWith("EXISTS_"):
                ApplyExistsFieldParameter(FieldType.CustomField, false, param.FieldName(), param.FieldNamespace()); break;
            case var s when s.StartsWith("EQ_INNER_"):
                ApplyFieldParameter(param.Values, FieldType.CustomField, true, param.InnerFieldName(), param.InnerFieldNamespace()); break;
            case var s when s.StartsWith("EQ_value_"):
                ApplyReportUomParameter(param.Values.Select(float.Parse).Cast<float?>().ToArray(), param.ReportFieldUom()); break;
            case var s when s.StartsWith("EQ_"):
                ApplyFieldParameter(param.Values, FieldType.CustomField, false, param.FieldName(), param.FieldNamespace()); break;
            case var s when s.StartsWith("EQATTR_"):
                ApplyEqAttributeParameter(param.Values, param.MasterdataType(), param.AttributeName()); break;
            case var s when s.StartsWith("HASATTR_"):
                ApplyHasAttributeParameter(param.MasterdataType(), param.AttributeName()); break;
            // Regex filters (Date/Numeric value comparison)
            case var r when Regexs.InnerIlmd().IsMatch(r):
                ApplyComparison(param, FieldType.Ilmd, param.InnerIlmdNamespace(), param.InnerIlmdName(), true); break;
            case var r when Regexs.Ilmd().IsMatch(r):
                ApplyComparison(param, FieldType.Ilmd, param.IlmdNamespace(), param.IlmdName(), false); break;
            case var r when Regexs.SensorFilter().IsMatch(r):
                ApplyComparison(param, param.SensorType(), param.SensorFieldNamespace(), param.SensorFieldName(), false); break;
            case var r when Regexs.InnerSensorFilter().IsMatch(r):
                ApplyComparison(param, param.InnerSensorType(), param.InnerSensorFieldNamespace(), param.InnerSensorFieldName(), true); break;
            case var r when Regexs.InnerField().IsMatch(r):
                ApplyComparison(param, FieldType.Extension, param.InnerFieldNamespace(), param.InnerFieldName(), true); break;
            case var r when Regexs.UoMField().IsMatch(r):
                ApplyUomComparison(param); break;
            case var r when Regexs.Field().IsMatch(r):
                ApplyComparison(param, FieldType.Extension, param.FieldNamespace(), param.FieldName(), false); break;
            // Any other case is an unknown parameter and should raise a QueryParameter Exception
            default:
                throw new EpcisException(ExceptionType.QueryParameterException, $"Parameter is not implemented: {param.Name}");
        }
    }

    public IQueryable<Event> Apply(IQueryable<Event> query)
    {
        return Paginate(Filter(query));
    }

    public IQueryable<Event> Filter(IQueryable<Event> query)
    {
        return _filters.Aggregate(query, (query, filter) => filter(query));
    }

    public IQueryable<Event> Paginate(IQueryable<Event> query)
    {
        return _pagination.Aggregate(query, (query, filter) => filter(query));
    }

    private void ApplyHasAttributeParameter(string field, string attributeName)
    {
        switch (field)
        {
            case "bizLocation":
                Filter(e => _context.Set<BizLocation>().Any(p => p.Id == e.BusinessLocation && p.Attributes.Any(a => a.Id == attributeName))); break;
            case "readPoint":
                Filter(e => _context.Set<ReadPoint>().Any(p => p.Id == e.ReadPoint && p.Attributes.Any(a => a.Id == attributeName))); break;
            default:
                throw new EpcisException(ExceptionType.QueryParameterException, $"Invalid masterdata field: {field}");
        }
    }

    private void ApplyEqAttributeParameter(string[] values, string field, string attributeName)
    {
        switch (field)
        {
            case "bizLocation":
                Filter(e => _context.Set<BizLocation>().Any(p => p.Id == e.BusinessLocation && p.Attributes.Any(a => a.Id == attributeName && values.Contains(a.Value)))); break;
            case "readPoint":
                Filter(e => _context.Set<ReadPoint>().Any(p => p.Id == e.ReadPoint && p.Attributes.Any(a => a.Id == attributeName && values.Contains(a.Value)))); break;
            default:
                throw new EpcisException(ExceptionType.QueryParameterException, $"Invalid masterdata field: {field}");
        }
    }

    private void ParseOrderField(QueryParameter param)
    {
        switch (param.AsString())
        {
            case "eventTime":
                _pagination.Add(x => _ascending ? x.OrderBy(x => x.EventTime) : x.OrderByDescending(x => x.EventTime)); break;
            case "recordTime":
                _pagination.Add(x => _ascending ? x.OrderBy(x => x.CaptureTime) : x.OrderByDescending(x => x.CaptureTime)); break;
            default:
                throw new EpcisException(ExceptionType.QueryParameterException, $"Invalid order field: {param.AsString()}");
        }
    }

    private void ApplyExistsFieldParameter(FieldType type, bool inner, string name, string ns)
    {
        Filter(x => x.Fields.Any(f => f.Type == type && f.ParentIndex == null == !inner && f.Name == name && f.Namespace == ns));
    }

    private void ApplyFieldParameter(string[] values, FieldType type, bool inner, string name, string ns)
    {
        Filter(x => x.Fields.Any(f => f.Type == type && f.ParentIndex == null == !inner && values.Contains(f.TextValue) && f.Name == name && f.Namespace == ns));
    }

    private void ApplyReportUomParameter(float?[] values, string uom)
    {
        Filter(x => x.SensorElements.Any(e => e.Reports.Any(r => r.UnitOfMeasure == uom && values.Contains(r.Value))));
    }

    private void ApplyComparison(QueryParameter param, FieldType type, string ns, string name, bool inner)
    {
        var customFieldPredicate = PredicateBuilder.New<Field>(f => f.Type == type && f.Name == name && f.Namespace == ns && f.ParentIndex != null == inner);
        var fieldValuePredicate = param.Name[..2] switch
        {
            "GE" => PredicateBuilder.New<Field>(param.IsDateTime() ? f => f.DateValue >= param.AsDate() : f => f.NumericValue >= param.AsFloat()),
            "GT" => PredicateBuilder.New<Field>(param.IsDateTime() ? f => f.DateValue > param.AsDate() : f => f.NumericValue > param.AsFloat()),
            "LE" => PredicateBuilder.New<Field>(param.IsDateTime() ? f => f.DateValue <= param.AsDate() : f => f.NumericValue <= param.AsFloat()),
            "LT" => PredicateBuilder.New<Field>(param.IsDateTime() ? f => f.DateValue < param.AsDate() : f => f.NumericValue < param.AsFloat()),
            _ => throw new EpcisException(ExceptionType.QueryParameterException, "Unknown Parameter")
        };

        Filter(x => x.Fields.AsQueryable().Any(customFieldPredicate.And(fieldValuePredicate)));
    }

    private void ApplyMatchParameter(QueryParameter param)
    {
        var epcType = param.GetMatchEpcTypes();
        var typePredicate = PredicateBuilder.New<Epc>(e => epcType.Contains(e.Type));
        var likePredicate = PredicateBuilder.New<Epc>();

        param.Values.Select(p => p.Replace("*", "%")).ForEach(value => likePredicate.Or(e => EF.Functions.Like(e.Id, value)));

        var finalPredicate = typePredicate.And(likePredicate);

        Filter(x => x.Epcs.AsQueryable().Any(finalPredicate));
    }

    private void ApplyPersistenDispositionFilter(QueryParameter param, PersistentDispositionType type)
    {
        var typePredicate = PredicateBuilder.New<PersistentDisposition>(x => x.Type == type);
        var anyPredicate = PredicateBuilder.New<PersistentDisposition>();
        param.Values.ForEach(value => anyPredicate.Or(e => e.Id == value));

        var finalPredicate = typePredicate.And(anyPredicate);

        Filter(x => x.PersistentDispositions.AsQueryable().Any(finalPredicate));
    }

    private void ApplyUomComparison(QueryParameter param)
    {
        var reportPredicate = PredicateBuilder.New<SensorReport>(r => r.UnitOfMeasure == param.ReportFieldUom());
        var fieldValuePredicate = param.Name[..2] switch
        {
            "GE" => PredicateBuilder.New<SensorReport>(r => EF.Property<float?>(r, param.ReportField()) >= param.AsFloat()),
            "GT" => PredicateBuilder.New<SensorReport>(r => EF.Property<float?>(r, param.ReportField()) > param.AsFloat()),
            "LE" => PredicateBuilder.New<SensorReport>(r => EF.Property<float?>(r, param.ReportField()) <= param.AsFloat()),
            "LT" => PredicateBuilder.New<SensorReport>(r => EF.Property<float?>(r, param.ReportField()) < param.AsFloat()),
            _ => throw new EpcisException(ExceptionType.QueryParameterException, "Unknown Parameter")
        };

        Filter(x => x.SensorElements.Any(x => x.Reports.AsQueryable().Any(reportPredicate.And(fieldValuePredicate))));
    }

    private void Filter(Expression<Func<Event, bool>> expression)
    {
        _filters.Add(evt => evt.Where(expression));
    }
}