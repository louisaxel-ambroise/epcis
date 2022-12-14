using FasTnT.Application.Database;
using FasTnT.Application.Services.DataSources.Utils;
using FasTnT.Domain;
using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Masterdata;
using FasTnT.Domain.Model.Queries;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FasTnT.Application.Services.DataSources;

public class EventDataSource : IEpcisDataSource
{
    const string ReadPoint = "urn:epcglobal:epcis:vtype:ReadPoint";
    const string Location = "urn:epcglobal:epcis:vtype:BusinessLocation";

    private bool _isMaxCount = false;
    private int _startFrom = 0;
    private int _eventCountLimit = Constants.Instance.MaxEventsReturnedInQuery;
    private OrderDirection _orderDirection = OrderDirection.Ascending;
    private Expression<Func<Event, object>> _orderExpression = e => e.CaptureTime;
    private readonly EpcisContext _context;

    public IQueryable<Event> Query { get; private set; }

    public EventDataSource(EpcisContext context)
    {
        _context = context;
        Query = _context.Set<Event>().AsNoTracking();
    }

    public void ApplyParameters(IEnumerable<QueryParameter> parameters)
    {
        foreach (var parameter in parameters)
        {
            try
            {
                Query = ApplyParameter(parameter, Query);
            }
            catch
            {
                throw new EpcisException(ExceptionType.QueryParameterException, $"Invalid Query Parameter or Value: {parameter.Name}");
            }
        }
    }

    public async Task<QueryData> ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            var eventIds = await ApplyOrderBy(Query)
                .Skip(_startFrom).Take(_eventCountLimit)
                .Select(evt => evt.Id)
                .ToListAsync(cancellationToken);

            if (_isMaxCount && eventIds.Count == _eventCountLimit)
            {
                throw new EpcisException(ExceptionType.QueryTooLargeException, $"Query returned too many events.");
            }

            var result = await _context.Set<Event>().AsNoTracking()
                .Where(evt => eventIds.Contains(evt.Id))
                .ToListAsync(cancellationToken);

            return ApplyOrderBy(result.AsQueryable()).ToList();
        }
        catch (InvalidOperationException ex) when (ex.InnerException is FormatException)
        {
            throw new EpcisException(ExceptionType.QueryParameterException, "Invalid parameter value.");
        }
        catch (Exception ex) when (ex is not EpcisException)
        {
            throw new EpcisException(ExceptionType.QueryTooComplexException, "Query too complex to be executed on this server.");
        }
    }

    private IQueryable<Event> ApplyOrderBy(IQueryable<Event> query)
    {
        return _orderDirection == OrderDirection.Ascending
            ? query.OrderBy(_orderExpression)
            : query.OrderByDescending(_orderExpression);
    }

    private IQueryable<Event> ApplyParameter(QueryParameter param, IQueryable<Event> query)
    {
        return param.Name switch
        {
            // Order Parameters
            "orderBy" => ParseOrderField(param, query),
            "orderDirection" => ParseOrderDirection(param, query),
            // Simple filters
            "eventType" => query.Where(x => param.Values.Select(x => Enum.Parse<EventType>(x, true)).Contains(x.Type)),
            "nextPageToken" => ParseNextPageToken(param, query),
            "eventCountLimit" or "perPage" or "maxEventCount" => ParseLimitEventCount(param, query),
            "GE_eventTime" => query.Where(x => x.EventTime >= param.GetDate()),
            "LT_eventTime" => query.Where(x => x.EventTime < param.GetDate()),
            "GE_recordTime" => query.Where(x => x.CaptureTime >= param.GetDate()),
            "LT_recordTime" => query.Where(x => x.CaptureTime < param.GetDate()),
            "EQ_action" => query.Where(x => param.Values.Select(x => Enum.Parse<EventAction>(x, true)).Contains(x.Action)),
            "EQ_bizLocation" => query.Where(x => param.Values.Contains(x.BusinessLocation)),
            "EQ_bizStep" => query.Where(x => param.Values.Contains(x.BusinessStep)),
            "EQ_disposition" => query.Where(x => param.Values.Contains(x.Disposition)),
            "EQ_eventID" => query.Where(x => param.Values.Contains(x.EventId)),
            "EQ_transformationID" => query.Where(x => param.Values.Contains(x.TransformationId)),
            "EQ_readPoint" => query.Where(x => param.Values.Contains(x.ReadPoint)),
            "EQ_userID" => query.Where(x => param.Values.Contains(x.Request.UserId)),
            "EXISTS_errorDeclaration" => query.Where(x => x.CorrectiveDeclarationTime.HasValue),
            "EQ_errorReason" => query.Where(x => param.Values.Contains(x.CorrectiveReason)),
            "EQ_correctiveEventID" => query.Where(x => x.CorrectiveEventIds.Any(ce => param.Values.Contains(ce.CorrectiveId))),
            "WD_readPoint" => query.Where(x => _context.Set<MasterDataHierarchy>().Where(h => h.Type == ReadPoint && h.Root == x.ReadPoint).Any(h => param.Values.Contains(h.Id))),
            "WD_bizLocation" => query.Where(x => _context.Set<MasterDataHierarchy>().Where(h => h.Type == Location && h.Root == x.BusinessLocation).Any(h => param.Values.Contains(h.Id))),
            "EQ_requestID" => query.Where(x => param.Values.Select(int.Parse).Contains(x.Request.Id)),
            "EQ_captureID" => query.Where(x => param.Values.Contains(x.Request.CaptureId)),
            "EQ_quantity" => query.Where(x => x.Epcs.Any(e => e.Type == EpcType.Quantity && e.Quantity == param.GetNumeric())),
            "GT_quantity" => query.Where(x => x.Epcs.Any(e => e.Type == EpcType.Quantity && e.Quantity > param.GetNumeric())),
            "GE_quantity" => query.Where(x => x.Epcs.Any(e => e.Type == EpcType.Quantity && e.Quantity >= param.GetNumeric())),
            "LT_quantity" => query.Where(x => x.Epcs.Any(e => e.Type == EpcType.Quantity && e.Quantity < param.GetNumeric())),
            "LE_quantity" => query.Where(x => x.Epcs.Any(e => e.Type == EpcType.Quantity && e.Quantity <= param.GetNumeric())),
            // parameters introduced in EPCIS 2.0
            "GE_startTime" => query.Where(x => x.SensorElements.Any(s => s.StartTime >= param.GetDate())),
            "LT_startTime" => query.Where(x => x.SensorElements.Any(s => s.StartTime < param.GetDate())),
            "GE_endTime" => query.Where(x => x.SensorElements.Any(s => s.EndTime >= param.GetDate())),
            "LT_endTime" => query.Where(x => x.SensorElements.Any(s => s.EndTime < param.GetDate())),
            "EQ_type" => query.Where(x => x.SensorElements.Any(s => s.Reports.Any(r => r.Type == param.Value()))),
            "EQ_deviceID" => query.Where(x => x.SensorElements.Any(s => s.Reports.Any(r => r.DeviceId == param.Value()))),
            "EQ_dataProcessingMethod" => query.Where(x => x.SensorElements.Any(s => s.Reports.Any(r => param.Values.Contains(r.DataProcessingMethod)))),
            "EQ_microorganism" => query.Where(x => x.SensorElements.Any(s => s.Reports.Any(r => param.Values.Contains(r.Microorganism)))),
            "EQ_chemicalSubstance" => query.Where(x => x.SensorElements.Any(s => s.Reports.Any(r => param.Values.Contains(r.ChemicalSubstance)))),
            "EQ_bizRules" => query.Where(x => x.SensorElements.Any(s => param.Values.Contains(s.BizRules))),
            "EQ_stringValue" => query.Where(x => x.SensorElements.Any(e => e.Reports.Any(r => r.StringValue == param.Value()))),
            "EQ_booleanValue" => query.Where(x => x.SensorElements.Any(e => e.Reports.Any(r => r.BooleanValue == param.GetBoolValue()))),
            "EQ_hexBinaryValue" => query.Where(x => x.SensorElements.Any(e => e.Reports.Any(r => param.Values.Contains(r.HexBinaryValue)))),
            "EQ_uriValue" => query.Where(x => x.SensorElements.Any(e => e.Reports.Any(r => param.Values.Contains(r.UriValue)))),
            "GE_percRank" => query.Where(x => x.SensorElements.Any(s => s.Reports.Any(r => r.PercRank >= param.GetNumeric()))),
            "LT_percRank" => query.Where(x => x.SensorElements.Any(s => s.Reports.Any(r => r.PercRank < param.GetNumeric()))),
            "EQ_persistentDisposition_set" => ApplyPersistenDispositionFilter(param, PersistentDispositionType.Set, query),
            "EQ_persistentDisposition_unset" => ApplyPersistenDispositionFilter(param, PersistentDispositionType.Unset, query),
            // Family filters
            var s when s.StartsWith("MATCH_") => ApplyMatchParameter(param, query),
            var s when s.StartsWith("EQ_source_") => query.Where(x => x.Sources.Any(s => s.Id == param.GetSimpleId() && param.Values.Contains(s.Type))),
            var s when s.StartsWith("EQ_destination_") => query.Where(x => x.Destinations.Any(d => d.Id == param.GetSimpleId() && param.Values.Contains(d.Type))),
            var s when s.StartsWith("EQ_bizTransaction_") => query.Where(x => x.Transactions.Any(t => t.Id == param.GetSimpleId() && param.Values.Contains(t.Type))),
            var s when s.StartsWith("EQ_INNER_ILMD_") => ApplyFieldParameter(param.Values, query, FieldType.Ilmd, true, param.InnerIlmdName(), param.InnerIlmdNamespace()),
            var s when s.StartsWith("EQ_ILMD_") => ApplyFieldParameter(param.Values, query, FieldType.Ilmd, false, param.IlmdName(), param.IlmdNamespace()),
            var s when s.StartsWith("EQ_INNER_SENSORELEMENT_") => ApplyFieldParameter(param.Values, query, FieldType.Sensor, true, param.InnerIlmdName(), param.InnerIlmdNamespace()),
            var s when s.StartsWith("EQ_SENSORELEMENT_") => ApplyFieldParameter(param.Values, query, FieldType.Sensor, false, param.IlmdName(), param.IlmdNamespace()),
            var s when s.StartsWith("EQ_SENSORMETADATA_") => ApplyFieldParameter(param.Values, query, FieldType.SensorMetadata, false, param.IlmdName(), param.IlmdNamespace()),
            var s when s.StartsWith("EQ_INNER_SENSORMETADATA_") => ApplyFieldParameter(param.Values, query, FieldType.SensorMetadata, true, param.InnerIlmdName(), param.InnerIlmdNamespace()),
            var s when s.StartsWith("EQ_SENSOREPORT_") => ApplyFieldParameter(param.Values, query, FieldType.SensorReport, false, param.IlmdName(), param.IlmdNamespace()),
            var s when s.StartsWith("EQ_INNER_SENSOREPORT_") => ApplyFieldParameter(param.Values, query, FieldType.SensorReport, true, param.InnerIlmdName(), param.InnerIlmdNamespace()),
            var s when s.StartsWith("EXISTS_INNER_ILMD_") => ApplyExistsFieldParameter(query, FieldType.Ilmd, true, param.InnerIlmdName(), param.InnerIlmdNamespace()),
            var s when s.StartsWith("EXISTS_ILMD_") => ApplyExistsFieldParameter(query, FieldType.Ilmd, false, param.IlmdName(), param.IlmdNamespace()),
            var s when s.StartsWith("EXISTS_INNER_") => ApplyExistsFieldParameter(query, FieldType.CustomField, true, param.InnerFieldName(), param.InnerFieldNamespace()),
            var s when s.StartsWith("EXISTS_") => ApplyExistsFieldParameter(query, FieldType.CustomField, false, param.FieldName(), param.FieldNamespace()),
            var s when s.StartsWith("EQ_INNER_") => ApplyFieldParameter(param.Values, query, FieldType.CustomField, true, param.InnerFieldName(), param.InnerFieldNamespace()),
            var s when s.StartsWith("EQ_value_") => ApplyReportUomParameter(param.Values.Select(float.Parse).Cast<float?>().ToArray(), query, param.ReportFieldUom()),
            var s when s.StartsWith("EQ_") => ApplyFieldParameter(param.Values, query, FieldType.CustomField, false, param.FieldName(), param.FieldNamespace()),
            var s when s.StartsWith("EQATTR_") => ApplyEqAttributeParameter(param.Values, query, param.MasterdataType(), param.AttributeName()),
            var s when s.StartsWith("HASATTR_") => ApplyHasAttributeParameter(query, param.MasterdataType(), param.AttributeName()),
            // Regex filters (Date/Numeric value comparison)
            var r when Regexs.IsInnerIlmd(r) => ApplyComparison(param, query, FieldType.Ilmd, param.InnerIlmdNamespace(), param.InnerIlmdName(), true),
            var r when Regexs.IsIlmd(r) => ApplyComparison(param, query, FieldType.Ilmd, param.IlmdNamespace(), param.IlmdName(), false),
            var r when Regexs.IsSensorElement(r) => ApplyComparison(param, query, FieldType.Sensor, param.InnerFieldNamespace(), param.InnerFieldName(), false),
            var r when Regexs.IsInnerSensorElement(r) => ApplyComparison(param, query, FieldType.Sensor, param.InnerFieldNamespace(), param.InnerFieldName(), true),
            var r when Regexs.IsSensorMetadata(r) => ApplyComparison(param, query, FieldType.SensorMetadata, param.InnerFieldNamespace(), param.InnerFieldName(), false),
            var r when Regexs.IsInnerSensorMetadata(r) => ApplyComparison(param, query, FieldType.SensorMetadata, param.InnerFieldNamespace(), param.InnerFieldName(), true),
            var r when Regexs.IsSensorReport(r) => ApplyComparison(param, query, FieldType.SensorReport, param.InnerFieldNamespace(), param.InnerFieldName(), false),
            var r when Regexs.IsInnerSensorReport(r) => ApplyComparison(param, query, FieldType.SensorReport, param.InnerFieldNamespace(), param.InnerFieldName(), true),
            var r when Regexs.IsInnerField(r) => ApplyComparison(param, query, FieldType.Extension, param.InnerFieldNamespace(), param.InnerFieldName(), true),
            var r when Regexs.IsUoMField(r) => ApplyUomComparison(param, query),
            var r when Regexs.IsField(r) => ApplyComparison(param, query, FieldType.Extension, param.FieldNamespace(), param.FieldName(), false),
            // Any other case is an unknown parameter and should raise a QueryParameter Exception
            _ => throw new EpcisException(ExceptionType.QueryParameterException, $"Parameter is not implemented: {param.Name}")
        };
    }

    private IQueryable<Event> ApplyHasAttributeParameter(IQueryable<Event> query, string field, string attributeName)
    {
        return field switch
        {
            "bizLocation" => query.Where(e => _context.Set<MasterData>().Any(p => p.Id == e.BusinessLocation && p.Type == Location && p.Attributes.Any(a => a.Id == attributeName))),
            "readPoint" => query.Where(e => _context.Set<MasterData>().Any(p => p.Id == e.ReadPoint && p.Type == ReadPoint && p.Attributes.Any(a => a.Id == attributeName))),
            _ => throw new EpcisException(ExceptionType.QueryParameterException, $"Invalid masterdata field: {field}"),
        };
    }

    private IQueryable<Event> ApplyEqAttributeParameter(string[] values, IQueryable<Event> query, string field, string attributeName)
    {
        return field switch
        {
            "bizLocation" => query.Where(e => _context.Set<MasterData>().Any(p => p.Id == e.BusinessLocation && p.Type == Location && p.Attributes.Any(a => a.Id == attributeName && values.Contains(a.Value)))),
            "readPoint" => query.Where(e => _context.Set<MasterData>().Any(p => p.Id == e.ReadPoint && p.Type == ReadPoint && p.Attributes.Any(a => a.Id == attributeName && values.Contains(a.Value)))),
            _ => throw new EpcisException(ExceptionType.QueryParameterException, $"Invalid masterdata field: {field}"),
        };
    }

    private IQueryable<Event> ParseOrderField(QueryParameter param, IQueryable<Event> query)
    {
        _orderExpression = param.Value() switch
        {
            "eventTime" => (x) => x.EventTime,
            "recordTime" => (x) => x.CaptureTime,
            _ => throw new EpcisException(ExceptionType.QueryParameterException, $"Invalid order field: {param.Value()}")
        };

        return query;
    }

    private IQueryable<Event> ParseNextPageToken(QueryParameter param, IQueryable<Event> query)
    {
        _startFrom = param.GetIntValue();

        return query;
    }

    private IQueryable<Event> ParseOrderDirection(QueryParameter param, IQueryable<Event> query)
    {
        _orderDirection = param.Value() switch
        {
            "ASC" => OrderDirection.Ascending,
            "DESC" => OrderDirection.Descending,
            _ => throw new EpcisException(ExceptionType.QueryParameterException, $"Invalid order direction: {param.Value()}")
        };

        return query;
    }

    private static IQueryable<Event> ApplyExistsFieldParameter(IQueryable<Event> query, FieldType type, bool inner, string name, string ns)
    {
        return query.Where(x => x.Fields.Any(f => f.Type == type && f.ParentIndex == null == !inner && f.Name == name && f.Namespace == ns));
    }

    private static IQueryable<Event> ApplyFieldParameter(string[] values, IQueryable<Event> query, FieldType type, bool inner, string name, string ns)
    {
        return query.Where(x => x.Fields.Any(f => f.Type == type && f.ParentIndex == null == !inner && values.Contains(f.TextValue) && f.Name == name && f.Namespace == ns));
    }

    private static IQueryable<Event> ApplyReportUomParameter(float?[] values, IQueryable<Event> query, string uom)
    {
        return query.Where(x => x.SensorElements.Any(e => e.Reports.Any(r => r.UnitOfMeasure == uom && values.Contains(r.Value))));
    }

    private IQueryable<Event> ParseLimitEventCount(QueryParameter param, IQueryable<Event> query)
    {
        _eventCountLimit = param.GetIntValue();
        _isMaxCount = param.Name == "maxEventCount";

        return query;
    }

    private static IQueryable<Event> ApplyComparison(QueryParameter param, IQueryable<Event> query, FieldType type, string ns, string name, bool inner)
    {
        var customFieldPredicate = PredicateBuilder.New<Field>(f => f.Type == type && f.Name == name && f.Namespace == ns && f.ParentIndex != null == inner);
        var fieldValuePredicate = param.Name[..2] switch
        {
            "GE" => PredicateBuilder.New<Field>(param.IsDateTime() ? f => f.DateValue >= param.GetDate() : f => f.NumericValue >= param.GetNumeric()),
            "GT" => PredicateBuilder.New<Field>(param.IsDateTime() ? f => f.DateValue > param.GetDate() : f => f.NumericValue > param.GetNumeric()),
            "LE" => PredicateBuilder.New<Field>(param.IsDateTime() ? f => f.DateValue <= param.GetDate() : f => f.NumericValue <= param.GetNumeric()),
            "LT" => PredicateBuilder.New<Field>(param.IsDateTime() ? f => f.DateValue < param.GetDate() : f => f.NumericValue < param.GetNumeric()),
            _ => throw new EpcisException(ExceptionType.QueryParameterException, "Unknown Parameter")
        };

        return query.Where(x => x.Fields.AsQueryable().Any(customFieldPredicate.And(fieldValuePredicate)));
    }

    private static IQueryable<Event> ApplyMatchParameter(QueryParameter param, IQueryable<Event> query)
    {
        var typePredicate = PredicateBuilder.New<Epc>(e => param.GetMatchEpcTypes().Contains(e.Type));
        var likePredicate = PredicateBuilder.New<Epc>();

        param.Values.Select(p => p.Replace("*", "%")).ForEach(value => likePredicate.Or(e => EF.Functions.Like(e.Id, value)));

        var finalPredicate = typePredicate.And(likePredicate);

        return query.Where(x => x.Epcs.AsQueryable().Any(finalPredicate));
    }

    private static IQueryable<Event> ApplyPersistenDispositionFilter(QueryParameter param, PersistentDispositionType type, IQueryable<Event> query)
    {
        var typePredicate = PredicateBuilder.New<PersistentDisposition>(x => x.Type == type);
        var anyPredicate = PredicateBuilder.New<PersistentDisposition>();
        param.Values.ForEach(value => anyPredicate.Or(e => e.Id == value));

        var finalPredicate = typePredicate.And(anyPredicate);

        return query.Where(x => x.PersistentDispositions.AsQueryable().Any(finalPredicate));
    }

    private static IQueryable<Event> ApplyUomComparison(QueryParameter param, IQueryable<Event> query)
    {
        var reportPredicate = PredicateBuilder.New<SensorReport>(r => r.UnitOfMeasure == param.ReportFieldUom());
        var fieldValuePredicate = param.Name[..2] switch
        {
            "GE" => PredicateBuilder.New<SensorReport>(r => EF.Property<float?>(r, param.ReportField()) >= param.GetNumeric()),
            "GT" => PredicateBuilder.New<SensorReport>(r => EF.Property<float?>(r, param.ReportField()) > param.GetNumeric()),
            "LE" => PredicateBuilder.New<SensorReport>(r => EF.Property<float?>(r, param.ReportField()) <= param.GetNumeric()),
            "LT" => PredicateBuilder.New<SensorReport>(r => EF.Property<float?>(r, param.ReportField()) < param.GetNumeric()),
            _ => throw new EpcisException(ExceptionType.QueryParameterException, "Unknown Parameter")
        };

        return query.Where(x => x.SensorElements.Any(x => x.Reports.AsQueryable().Any(reportPredicate.And(fieldValuePredicate))));
    }
}
