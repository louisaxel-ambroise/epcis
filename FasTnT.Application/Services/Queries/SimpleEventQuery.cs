using FasTnT.Application.Services;
using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;
using FasTnT.Domain.Utils;
using FasTnT.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Application.Queries.Poll
{
    public class SimpleEventQuery : IEpcisQuery
    {
        private static readonly IDictionary<string, Func<IQueryable<Event>, QueryParameter, IQueryable<Event>>> SimpleParameters = new Dictionary<string, Func<IQueryable<Event>, QueryParameter, IQueryable<Event>>>
        {
            { "eventType",               (query, param) => query.Where(x => param.Values.Select(Enumeration.GetByDisplayName<EventType>).Contains(x.Type)) },
            { "eventCountLimit",         (query, param) => query.Take(param.GetValue<int>()) },
            { "maxEventCount",           (query, param) => query.Take(param.GetValue<int>() + 1) },
            { "EQ_action",               (query, param) => query.Where(x => param.Values.Select(Enumeration.GetByDisplayName<EventAction>).Contains(x.Action)) },
            { "EQ_bizLocation",          (query, param) => query.Where(x => param.Values.Contains(x.BusinessLocation)) },
            { "EQ_bizStep",              (query, param) => query.Where(x => param.Values.Contains(x.BusinessStep)) },
            { "EQ_disposition",          (query, param) => query.Where(x => param.Values.Contains(x.Disposition)) },
            { "EQ_eventID",              (query, param) => query.Where(x => param.Values.Contains(x.EventId)) },
            { "EQ_transformationID",     (query, param) => query.Where(x => param.Values.Contains(x.TransformationId)) },
            { "EQ_readPoint",            (query, param) => query.Where(x => param.Values.Contains(x.ReadPoint)) },
            { "EXISTS_errorDeclaration", (query, param) => query.Where(x => x.CorrectiveDeclarationTime.HasValue) },
            { "EQ_errorReason",          (query, param) => query.Where(x => param.Values.Contains(x.CorrectiveReason)) },
            { "EQ_correctiveEventID",    (query, param) => query.Where(x => x.CorrectiveEventIds.Any(ce => param.Values.Contains(ce.CorrectiveId))) },
            //{ "WD_readPoint",            (query, param) => query.Where(new MasterdataHierarchyFilter { Field = EpcisField.ReadPoint, Values = param.Values }) },
            //{ "WD_bizLocation",          (query, param) => query.Where(new MasterdataHierarchyFilter { Field = EpcisField.BusinessLocation, Values = param.Values }) },
            { "EQ_requestId",            (query, param) => query.Where(x => param.Values.Select(int.Parse).Contains(x.Request.Id)) },
            //{ "orderBy",                 (query, param) => query.Where(new OrderFilter { Field = Enumeration.GetByDisplayName<EpcisField>(param.GetValue<string>()) }) },
            //{ "orderDirection",          (query, param) => query.Where(new OrderDirectionFilter { Direction = Enumeration.GetByDisplayName<OrderDirection>(param.GetValue<string>()) }) }
        };
        private static readonly IDictionary<string, Func<IQueryable<Event>, QueryParameter, IQueryable<Event>>> RegexParameters = new Dictionary<string, Func<IQueryable<Event>, QueryParameter, IQueryable<Event>>>
        {
            { "^EQ_(source|destination)_",  (query, param) => query.Where(x => x.SourceDests.Any(sd => sd.Id == param.GetParamNameValue('_', 3, 2) && sd.Direction == param.GetSourceDestinationType() && param.Values.Contains(sd.Type))) },
            { "^EQ_bizTransaction_",        (query, param) => query.Where(x => x.Transactions.Any(t => t.Id == param.GetParamNameValue('_', 3, 2) && param.Values.Contains(t.Id))) },
            //{ "^(GE|LT)_eventTime",         (query, param) => query.Where(x => .Where(new ComparisonParameterFilter { Field = EpcisField.EventTime, Comparator = param.GetComparator(), Value = param.GetValue<DateTime>() }) },
            //{ "^(GE|LT)_recordTime",        (query, param) => query.Where(new ComparisonParameterFilter { Field = EpcisField.RecordTime, Comparator = param.GetComparator(), Value = param.GetValue<DateTime>() }) },
            //{ "^MATCH_",                    (query, param) => query.Where(new MatchEpcFilter { EpcType = param.GetMatchEpcTypes(), Values = param.Values.Select(x => x.Replace("*", "%")).ToArray() }) },
            //{ "^(EQ|GT|LT|GE|LE)_quantity$",(query, param) => query.Where(new QuantityFilter { Operator = param.GetComparator(), Value = param.GetValue<double>() }) },
            //{ "^EQ_INNER_ILMD_",            (query, param) => query.Where(new CustomFieldFilter { Field = param.GetField(FieldType.Ilmd, true), IsInner = true, Values = param.Values }) },
            //{ "^EQ_ILMD_",                  (query, param) => query.Where(new CustomFieldFilter { Field = param.GetField(FieldType.Ilmd, false), IsInner = false, Values = param.Values }) },
            //{ "^(GT|LT|GE|LE)_INNER_ILMD_", (query, param) => query.Where(new ComparisonCustomFieldFilter { Field = param.GetField(FieldType.Ilmd, true), Comparator = param.GetComparator(), IsInner = true, Value = param.GetComparisonValue()  }) },
            //{ "^(GT|LT|GE|LE)_ILMD_",       (query, param) => query.Where(new ComparisonCustomFieldFilter { Field = param.GetField(FieldType.Ilmd, false), Comparator = param.GetComparator(), IsInner = false, Value = param.GetComparisonValue()  }) },
            //{ "^EXISTS_INNER_ILMD_",        (query, param) => query.Where(new ExistCustomFieldFilter { Field = param.GetField(FieldType.Ilmd, true), IsInner = true }) },
            //{ "^EXISTS_ILMD_",              (query, param) => query.Where(new ExistCustomFieldFilter { Field = param.GetField(FieldType.Ilmd, false), IsInner = false }) },
            //{ "^EQ_INNER_",                 (query, param) => query.Where(new CustomFieldFilter { Field = param.GetField(FieldType.CustomField, true), IsInner = true, Values = param.Values }) },
            //{ "^EQ_",                       (query, param) => query.Where(new CustomFieldFilter { Field = param.GetField(FieldType.CustomField, false), IsInner = false, Values = param.Values }) },
            //{ "^(GT|LT|GE|LE)_INNER_",      (query, param) => query.Where(new ComparisonCustomFieldFilter { Field = param.GetField(FieldType.CustomField, true), Comparator = param.GetComparator(), IsInner = true, Value = param.GetComparisonValue() }) },
            //{ "^(GT|LT|GE|LE)_",            (query, param) => query.Where(new ComparisonCustomFieldFilter { Field = param.GetField(FieldType.CustomField, false), Comparator = param.GetComparator(), IsInner = false, Value = param.GetComparisonValue() }) },
            //{ "^EXISTS_INNER",              (query, param) => query.Where(new ExistCustomFieldFilter { Field = param.GetField(FieldType.CustomField, true), IsInner = true }) },
            //{ "^EQATTR_",                   (query, param) => query.Where(new AttributeFilter { Field = param.GetAttributeField(), AttributeName = param.GetAttributeName(), Values = param.Values }) },
            //{ "^HASATTR_",                  (query, param) => query.Where(new ExistsAttributeFilter { Field = param.GetAttributeField(), AttributeName = param.GetAttributeName()}) }
        };
        private readonly EpcisContext _context;
        private int? _maxEventCount = default;

        public SimpleEventQuery(EpcisContext context)
        {
            _context = context;
        }

        public string Name => "SimpleEventQuery";
        public bool AllowSubscription => true;

        public async Task<PollResponse> HandleAsync(IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken)
        {
            var query = _context.Events.AsNoTracking();

            foreach (var parameter in parameters)
            {
                query = ApplyParameter(parameter, query);
            }

            var eventIds = await query.Select(evt => evt.Id).ToListAsync(cancellationToken);
            
            CheckEventCountRestriction(eventIds);
            
            var result = await _context.Events.AsSplitQuery()
                .Include(x => x.Epcs)
                .Include(x => x.SourceDests)
                .Include(x => x.CustomFields).ThenInclude(x => x.Children)
                .Include(x => x.Transactions)
                .Where(evt => eventIds.Contains(evt.Id))
                .ToListAsync(cancellationToken);

            return new PollResponse
            {
                QueryName = Name,
                EventList = result.ToArray()
            };
        }

        private IQueryable<Event> ApplyParameter(QueryParameter parameter, IQueryable<Event> query)
        {
            if (IsSimpleParameter(parameter, out Func<IQueryable<Event>, QueryParameter, IQueryable<Event>> action) || IsRegexParameter(parameter, out action))
            {
                try
                {
                    if (parameter.Name == "maxEventCount")
                    {
                        _maxEventCount = parameter.GetValue<int>();
                    }

                    return action(query, parameter);
                }
                catch (Exception ex)
                {
                    throw new EpcisException(ExceptionType.QueryParameterException, ex.Message);
                }
            }
            
            throw new NotImplementedException($"Query parameter unexpected or not implemented: '{parameter.Name}'");
        }

        private static bool IsSimpleParameter(QueryParameter parameter, out Func<IQueryable<Event>, QueryParameter, IQueryable<Event>> action)
        {
            return SimpleParameters.TryGetValue(parameter.Name, out action);
        }

        private static bool IsRegexParameter(QueryParameter parameter, out Func<IQueryable<Event>, QueryParameter, IQueryable<Event>> action)
        {
            var matchingRegex = RegexParameters.FirstOrDefault(x => Regex.Match(parameter.Name, x.Key, RegexOptions.Singleline).Success);
            action = matchingRegex.Value;

            return matchingRegex.Key != default;
        }

        private void CheckEventCountRestriction(IEnumerable<long> result)
        {
            if (_maxEventCount.HasValue && result.Count() > _maxEventCount)
            {
                throw new EpcisException(ExceptionType.QueryTooLargeException, $"Query returned more than the {_maxEventCount} events allowed.");
            }
        }
    }
}
