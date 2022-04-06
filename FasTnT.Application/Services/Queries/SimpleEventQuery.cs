using FasTnT.Application.Services;
using FasTnT.Domain;
using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;
using FasTnT.Domain.Queries;
using FasTnT.Domain.Utils;
using FasTnT.Infrastructure.Database;
using LinqKit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace FasTnT.Application.Queries;

public class SimpleEventQuery : IEpcisQuery
{
    const string Comparison = "(GE|GT|LE|LT)";

    private readonly EpcisContext _context;
    private int? _maxEventCount = default, _eventCountLimit = Constants.MaxEventsReturnedInQuery + 1;
    private OrderDirection _orderDirection = OrderDirection.Ascending;
    private Expression<Func<Event, object>> _orderExpression = e => e.CaptureTime;

    public SimpleEventQuery(EpcisContext context)
    {
        _context = context;
    }

    public string Name => nameof(SimpleEventQuery);
    public bool AllowSubscription => true;

    public async Task<PollResponse> HandleAsync(IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken)
    {
        var query = _context.Events.AsNoTracking();

        foreach (var parameter in parameters)
        {
            try
            {
                query = ApplyParameter(parameter, query);
            }
            catch
            {
                throw new EpcisException(ExceptionType.QueryParameterException, $"Invalid Query Parameter or Value: {parameter.Name}");
            }
        }

        try
        {
            var eventIds = await ApplyOrderByLimit(query)
                .Select(evt => evt.Id)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            if (_maxEventCount.HasValue && eventIds.Count > _maxEventCount || eventIds.Count > Constants.MaxEventsReturnedInQuery)
            {
                throw new EpcisException(ExceptionType.QueryTooLargeException, $"Query returned too many events.")
                {
                    QueryName = Name
                };
            }
            
            if (eventIds.Count > 0)
            {
                query = _context.Events.AsSplitQuery().AsNoTrackingWithIdentityResolution()
                    .Include(x => x.Epcs)
                    .Include(x => x.Sources)
                    .Include(x => x.Destinations)
                    .Include(x => x.CustomFields)
                    .Include(x => x.Transactions)
                    .Where(evt => eventIds.Contains(evt.Id));

                var result = await query.ToListAsync(cancellationToken)
                    .ContinueWith(x => ApplyOrderByLimit(x.Result.AsQueryable()).ToList());

                return new PollEventResponse(Name, result);
            }
            else
            {
                return new PollEventResponse(Name, new ());
            }
        }
        catch (InvalidOperationException ex) when (ex.InnerException is FormatException)
        {
            throw new EpcisException(ExceptionType.QueryParameterException, "Invalid parameter value.");
        }
        catch (SqlException e) when (e.Number == -2)
        {
            throw new EpcisException(ExceptionType.ImplementationException, "Query took too long to execute")
            {
                Severity = ExceptionSeverity.Severe,
                QueryName = Name
            };
        }
    }

    private IQueryable<Event> ApplyOrderByLimit(IQueryable<Event> query)
    {
        var limit = _maxEventCount.HasValue 
            ? _maxEventCount.Value + 1
            : _eventCountLimit.Value;    

        return _orderDirection == OrderDirection.Ascending
            ? query.OrderBy(_orderExpression).Take(limit)
            : query.OrderByDescending(_orderExpression).Take(limit);
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
            "eventCountLimit" => ParseLimitEventCount(param, query, ref _eventCountLimit),
            "maxEventCount" => ParseLimitEventCount(param, query, ref _maxEventCount),
            "GE_eventTime" => query.Where(x => x.EventTime >= param.GetDate()),
            "LT_eventTime" => query.Where(x => x.EventTime < param.GetDate()),
            "GE_recordTime" => query.Where(x => x.Request.CaptureDate >= param.GetDate()),
            "LT_recordTime" => query.Where(x => x.Request.CaptureDate < param.GetDate()),
            "EQ_action" => query.Where(x => param.Values.Select(x => Enum.Parse<EventAction>(x, true)).Contains(x.Action)),
            "EQ_bizLocation" => query.Where(x => param.Values.Contains(x.BusinessLocation)),
            "EQ_bizStep" => query.Where(x => param.Values.Contains(x.BusinessStep)),
            "EQ_disposition" => query.Where(x => param.Values.Contains(x.Disposition)),
            "EQ_eventID" => query.Where(x => param.Values.Contains(x.EventId)),
            "EQ_transformationID" => query.Where(x => param.Values.Contains(x.TransformationId)),
            "EQ_readPoint" => query.Where(x => param.Values.Contains(x.ReadPoint)),
            "EQ_username" => query.Where(x => param.Values.Contains(x.Request.User.Username)),
            "EXISTS_errorDeclaration" => query.Where(x => x.CorrectiveDeclarationTime.HasValue),
            "EQ_errorReason" => query.Where(x => param.Values.Contains(x.CorrectiveReason)),
            "EQ_correctiveEventID" => query.Where(x => x.CorrectiveEventIds.Any(ce => param.Values.Contains(ce.CorrectiveId))),
            "WD_readPoint" => throw new EpcisException(ExceptionType.ImplementationException, "WD_readPoint parameter is not implemented"),
            "WD_bizLocation" => throw new EpcisException(ExceptionType.ImplementationException, "WD_bizLocation parameter is not implemented"),
            "EQ_requestId" => query.Where(x => param.Values.Select(int.Parse).Contains(x.Request.Id)),
            "EQ_quantity" => query.Where(x => x.Epcs.Any(e => e.Type == EpcType.Quantity && e.Quantity == param.GetNumeric())),
            "GT_quantity" => query.Where(x => x.Epcs.Any(e => e.Type == EpcType.Quantity && e.Quantity > param.GetNumeric())),
            "GE_quantity" => query.Where(x => x.Epcs.Any(e => e.Type == EpcType.Quantity && e.Quantity >= param.GetNumeric())),
            "LT_quantity" => query.Where(x => x.Epcs.Any(e => e.Type == EpcType.Quantity && e.Quantity < param.GetNumeric())),
            "LE_quantity" => query.Where(x => x.Epcs.Any(e => e.Type == EpcType.Quantity && e.Quantity <= param.GetNumeric())),
            // Family filters
            var s when s.StartsWith("MATCH_") => ApplyMatchParameter(param, query),
            var s when s.StartsWith("EQ_source_") => query.Where(x => x.Sources.Any(s => s.Id == param.GetSimpleId() && param.Values.Contains(s.Type))),
            var s when s.StartsWith("EQ_destination_") => query.Where(x => x.Destinations.Any(d => d.Id == param.GetSimpleId() && param.Values.Contains(d.Type))),
            var s when s.StartsWith("EQ_bizTransaction_") => query.Where(x => x.Transactions.Any(t => t.Id == param.GetSimpleId() && param.Values.Contains(t.Type))),
            var s when s.StartsWith("EQ_INNER_ILMD_") => ApplyCustomFieldParameter(param.Values, query, FieldType.Ilmd, true, param.InnerIlmdName(), param.InnerIlmdNamespace()),
            var s when s.StartsWith("EQ_ILMD_") => ApplyCustomFieldParameter(param.Values, query, FieldType.Ilmd, false, param.IlmdName(), param.IlmdNamespace()),
            var s when s.StartsWith("EXISTS_INNER_ILMD_") => ApplyExistsCustomFieldParameter(query, FieldType.Ilmd, true, param.InnerIlmdName(), param.InnerIlmdNamespace()),
            var s when s.StartsWith("EXISTS_ILMD_") => ApplyExistsCustomFieldParameter(query, FieldType.Ilmd, false, param.IlmdName(), param.IlmdNamespace()),
            var s when s.StartsWith("EXISTS_INNER_") => ApplyExistsCustomFieldParameter(query, FieldType.CustomField, true, param.InnerFieldName(), param.InnerFieldNamespace()),
            var s when s.StartsWith("EQ_INNER_") => ApplyCustomFieldParameter(param.Values, query, FieldType.CustomField, true, param.InnerFieldName(), param.InnerFieldNamespace()),
            var s when s.StartsWith("EQ_") => ApplyCustomFieldParameter(param.Values, query, FieldType.CustomField, false, param.FieldName(), param.FieldNamespace()),
            // Regex filters (Date/Numeric value comparison)
            var r when Regex.IsMatch(r, $"^{Comparison}_INNER_ILMD") => ApplyComparison(param, query, FieldType.Ilmd, param.InnerIlmdNamespace(), param.InnerIlmdName(), true),
            var r when Regex.IsMatch(r, $"^{Comparison}_ILMD") => ApplyComparison(param, query, FieldType.Ilmd, param.IlmdNamespace(), param.IlmdName(), false),
            var r when Regex.IsMatch(r, $"^{Comparison}_INNER") => ApplyComparison(param, query, FieldType.Extension, param.InnerFieldNamespace(), param.InnerFieldName(), true),
            var r when Regex.IsMatch(r, $"^{Comparison}_") => ApplyComparison(param, query, FieldType.Extension, param.FieldNamespace(), param.FieldName(), false),
            // Regex HasAttr/EqAttr filters
            var r when Regex.IsMatch(r, $"^EQATTR_") => throw new EpcisException(ExceptionType.ImplementationException, "EQATTR_ parameter family is not implemented"),
            var r when Regex.IsMatch(r, $"^HASATTR_") => throw new EpcisException(ExceptionType.ImplementationException, "HASATTR_ parameter family is not implemented"),
            // Any other case is an unknown parameter and should raise a QueryParameter Exception
            _ => throw new EpcisException(ExceptionType.QueryParameterException, $"Parameter is not implemented: {param.Name}")
        };
    }

    private IQueryable<Event> ParseOrderField(QueryParameter param, IQueryable<Event> query)
    {
        _orderExpression = param.Value() switch
        {
            "eventTime" => (Event x) => x.EventTime,
            "recordTime" => (Event x) => x.CaptureTime,
            _ => throw new EpcisException(ExceptionType.QueryParameterException, $"Invalid order field: {param.Value()}")
        };

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

    private static IQueryable<Event> ApplyExistsCustomFieldParameter(IQueryable<Event> query, FieldType type, bool inner, string name, string ns)
        => query.Where(x => x.CustomFields.Any(f => f.Type == type && f.Parent == null == !inner && f.Name == name && f.Namespace == ns));

    private static IQueryable<Event> ApplyCustomFieldParameter(string[] values, IQueryable<Event> query, FieldType type, bool inner, string name, string ns)
        => query.Where(x => x.CustomFields.Any(f => f.Type == type && f.Parent == null == !inner && values.Contains(f.TextValue) && f.Name == name && f.Namespace == ns));

    private static IQueryable<Event> ParseLimitEventCount(QueryParameter param, IQueryable<Event> query, ref int? destination)
    {
        destination = param.GetIntValue();

        return query;
    }

    private static IQueryable<Event> ApplyComparison(QueryParameter param, IQueryable<Event> query, FieldType type, string ns, string name, bool inner)
    {
        var customFieldPredicate = PredicateBuilder.New<CustomField>(f => f.Type == type && f.Name == name && f.Namespace == ns && f.HasParent == inner);
        var fieldValuePredicate = param.Name[..2] switch
        {
            "GE" => PredicateBuilder.New<CustomField>(param.IsDateTime() ? f => f.DateValue >= param.GetDate() : f => f.NumericValue >= param.GetNumeric()),
            "GT" => PredicateBuilder.New<CustomField>(param.IsDateTime() ? f => f.DateValue > param.GetDate() : f => f.NumericValue > param.GetNumeric()),
            "LE" => PredicateBuilder.New<CustomField>(param.IsDateTime() ? f => f.DateValue <= param.GetDate() : f => f.NumericValue <= param.GetNumeric()),
            "LT" => PredicateBuilder.New<CustomField>(param.IsDateTime() ? f => f.DateValue < param.GetDate() : f => f.NumericValue < param.GetNumeric()),
            _ => throw new EpcisException(ExceptionType.QueryParameterException, "Unknown Parameter")
        };

        return query.Where(x => x.CustomFields.AsQueryable().Any(customFieldPredicate.And(fieldValuePredicate)));
    }

    private static IQueryable<Event> ApplyMatchParameter(QueryParameter param, IQueryable<Event> query)
    {
        var typePredicate = PredicateBuilder.New<Epc>(e => param.GetMatchEpcTypes().Contains(e.Type));
        var likePredicate = PredicateBuilder.New<Epc>();

        param.Values.Select(p => p.Replace("*", "%")).ForEach(value => likePredicate.Or(e => EF.Functions.Like(e.Id, value)));

        var finalPredicate = typePredicate.And(likePredicate);

        return query.Where(x => x.Epcs.AsQueryable().Any(finalPredicate));
    }
}
