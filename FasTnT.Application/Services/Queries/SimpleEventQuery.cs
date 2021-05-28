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
        const string Comp = "(GE|GT|LE|LT)";

        private readonly EpcisContext _context;
        private int? _maxEventCount = default;

        public SimpleEventQuery(EpcisContext context)
        {
            _context = context;
        }

        public string Name => nameof(SimpleEventQuery);
        public bool AllowSubscription => true;

        public async Task<PollResponse> HandleAsync(IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken)
        {
            var query = _context.Events.AsNoTracking();

            // TODO: handle orderBy and orderDirection filters.
            foreach (var parameter in parameters)
            {
                query = ApplyParameter(parameter, query);
            }

            var eventIds = await query
                .Select(evt => evt.Id)
                .ToListAsync(cancellationToken);

            if (_maxEventCount.HasValue && eventIds.Count > _maxEventCount)
            {
                throw new EpcisException(ExceptionType.QueryTooLargeException, $"Query returned more than the {_maxEventCount} events allowed.");
            }

            var result = await _context.Events.AsSplitQuery().AsNoTrackingWithIdentityResolution()
                .Include(x => x.Epcs)
                .Include(x => x.Sources)
                .Include(x => x.Destinations)
                .Include(x => x.CustomFields).ThenInclude(x => x.Children)
                .Include(x => x.Transactions)
                .Where(evt => eventIds.Contains(evt.Id))
                .ToListAsync(cancellationToken);

            return new(Name, default, result.ToList(), new());
        }

        private IQueryable<Event> ApplyParameter(QueryParameter param, IQueryable<Event> query)
        {
            return param.Name switch
            {
                // Simple filters
                "eventType"               => query.Where(x => param.Values.Select(Enumeration.GetByDisplayName<EventType>).Contains(x.Type)),
                "eventCountLimit"         => query.Take(param.GetIntValue()),
                "maxEventCount"           => ParseMaxEventCount(param, query),
                "GE_eventTime"            => query.Where(x => x.EventTime >= param.GetDate()),
                "LT_eventTime"            => query.Where(x => x.EventTime < param.GetDate()),
                "GE_recordTime"           => query.Where(x => x.Request.CaptureDate >= param.GetDate()),
                "LT_recordTime"           => query.Where(x => x.Request.CaptureDate < param.GetDate()),
                "EQ_action"               => query.Where(x => param.Values.Select(Enumeration.GetByDisplayName<EventAction>).Contains(x.Action)),
                "EQ_bizLocation"          => query.Where(x => param.Values.Contains(x.BusinessLocation)),
                "EQ_bizStep"              => query.Where(x => param.Values.Contains(x.BusinessStep)),
                "EQ_disposition"          => query.Where(x => param.Values.Contains(x.Disposition)),
                "EQ_eventID"              => query.Where(x => param.Values.Contains(x.EventId)),
                "EQ_transformationID"     => query.Where(x => param.Values.Contains(x.TransformationId)),
                "EQ_readPoint"            => query.Where(x => param.Values.Contains(x.ReadPoint)),
                "EXISTS_errorDeclaration" => query.Where(x => x.CorrectiveDeclarationTime.HasValue),
                "EQ_errorReason"          => query.Where(x => param.Values.Contains(x.CorrectiveReason)),
                "EQ_correctiveEventID"    => query.Where(x => x.CorrectiveEventIds.Any(ce => param.Values.Contains(ce.CorrectiveId))),
                "WD_readPoint"            => throw new EpcisException(ExceptionType.ImplementationException, "WD_readPoint parameter is not implemented"),
                "WD_bizLocation"          => throw new EpcisException(ExceptionType.ImplementationException, "WD_bizLocation parameter is not implemented"),
                "EQ_requestId"            => query.Where(x => param.Values.Select(int.Parse).Contains(x.Request.Id)),
                "EQ_quantity"             => query.Where(x => x.Epcs.Any(e => e.Type == EpcType.Quantity && e.Quantity == param.GetNumeric())),
                "GT_quantity"             => query.Where(x => x.Epcs.Any(e => e.Type == EpcType.Quantity && e.Quantity > param.GetNumeric())),
                "GE_quantity"             => query.Where(x => x.Epcs.Any(e => e.Type == EpcType.Quantity && e.Quantity >= param.GetNumeric())),
                "LT_quantity"             => query.Where(x => x.Epcs.Any(e => e.Type == EpcType.Quantity && e.Quantity < param.GetNumeric())),
                "LE_quantity"             => query.Where(x => x.Epcs.Any(e => e.Type == EpcType.Quantity && e.Quantity <= param.GetNumeric())),
                // Family filters
                var s when s.StartsWith("MATCH_")             => ApplyMatchParameter(param, query),
                var s when s.StartsWith("EQ_source_")         => query.Where(x => x.Sources.Any(s => s.Id == param.GetSimpleId() && param.Values.Contains(s.Type))),
                var s when s.StartsWith("EQ_destination_")    => query.Where(x => x.Destinations.Any(d => d.Id == param.GetSimpleId() && param.Values.Contains(d.Type))),
                var s when s.StartsWith("EQ_bizTransaction_") => query.Where(x => x.Transactions.Any(t => t.Id == param.GetSimpleId() && param.Values.Contains(t.Type))),
                var s when s.StartsWith("EQ_INNER_ILMD_")     => query.Where(x => x.CustomFields.Any(f => f.Type == FieldType.Ilmd && f.Parent != null && param.Values.Contains(f.TextValue) && f.Name == param.InnerIlmdName() && f.Namespace == param.InnerIlmdNamespace())),
                var s when s.StartsWith("EQ_ILMD_")           => query.Where(x => x.CustomFields.Any(f => f.Type == FieldType.Ilmd && f.Parent == null && param.Values.Contains(f.TextValue) && f.Name == param.IlmdName() && f.Namespace == param.IlmdNamespace())),
                var s when s.StartsWith("EXISTS_INNER_ILMD_") => query.Where(x => x.CustomFields.Any(f => f.Type == FieldType.Ilmd && f.Parent != null && f.Name == param.InnerIlmdName() && f.Namespace == param.InnerIlmdNamespace())),
                var s when s.StartsWith("EXISTS_ILMD_")       => query.Where(x => x.CustomFields.Any(f => f.Type == FieldType.Ilmd && f.Parent == null && f.Name == param.IlmdName() && f.Namespace == param.IlmdNamespace())),
                var s when s.StartsWith("EXISTS_INNER_")      => query.Where(x => x.CustomFields.Any(f => f.Type == FieldType.CustomField && f.Parent != null && f.Name == param.InnerFieldName() && f.Namespace == param.InnerFieldNamespace())),
                var s when s.StartsWith("EQ_INNER_")          => query.Where(x => x.CustomFields.Any(f => f.Type == FieldType.CustomField && f.Parent != null && f.Name == param.InnerFieldName() && f.Namespace == param.InnerFieldNamespace() && param.Values.Contains(f.TextValue))),
                var s when s.StartsWith("EQ_")                => query.Where(x => x.CustomFields.Any(f => f.Type == FieldType.CustomField && f.Parent == null && f.Name == param.FieldName() && f.Namespace == param.FieldNamespace() && param.Values.Contains(f.TextValue))),
                // Regex filters (Date/Numeric value comparison)
                var r when Regex.IsMatch(r, $"^{Comp}_INNER_ILMD") => ApplyComparison(param, query, FieldType.Ilmd, param.InnerIlmdNamespace(), param.InnerIlmdName(), true),
                var r when Regex.IsMatch(r, $"^{Comp}_ILMD")       => ApplyComparison(param, query, FieldType.Ilmd, param.IlmdNamespace(), param.IlmdName(), false),
                var r when Regex.IsMatch(r, $"^{Comp}_INNER")      => ApplyComparison(param, query, FieldType.Extension, param.InnerFieldNamespace(), param.InnerFieldName(), true),
                var r when Regex.IsMatch(r, $"^{Comp}_")           => ApplyComparison(param, query, FieldType.Extension, param.FieldNamespace(), param.FieldName(), false),
                //{ "^EQATTR_",                   (query, param) => query.Where(new AttributeFilter { Field = param.GetAttributeField(), AttributeName = param.GetAttributeName(), Values = param.Values }) },
                //{ "^HASATTR_",                  (query, param) => query.Where(new ExistsAttributeFilter { Field = param.GetAttributeField(), AttributeName = param.GetAttributeName()}) }
                _ => throw new EpcisException(ExceptionType.QueryParameterException, $"Parameter is not implemented: {param.Name}")
            };
        }

        private IQueryable<Event> ParseMaxEventCount(QueryParameter param, IQueryable<Event> query)
        {
            _maxEventCount = param.GetIntValue();

            return query.Take(1 + _maxEventCount.Value);
        }

        private static IQueryable<Event> ApplyComparison(QueryParameter param, IQueryable<Event> query, FieldType type, string nameSpace, string name, bool inner)
        {
            return param.Name.Substring(0, 2) switch
            {
                "GE" => query.Where(x => x.CustomFields.Any(f => f.Type == type && f.Parent == null == !inner && f.Name == name && f.Namespace == nameSpace &&((param.IsDateTime() && f.DateValue >= param.GetDate()) || (param.IsNumeric() && f.NumericValue >= param.GetNumeric())))),
                "GT" => query.Where(x => x.CustomFields.Any(f => f.Type == type && f.Parent == null == !inner && f.Name == name && f.Namespace == nameSpace && ((param.IsDateTime() && f.DateValue > param.GetDate()) || (param.IsNumeric() && f.NumericValue > param.GetNumeric())))),
                "LE" => query.Where(x => x.CustomFields.Any(f => f.Type == type && f.Parent == null == !inner && f.Name == name && f.Namespace == nameSpace && ((param.IsDateTime() && f.DateValue <= param.GetDate()) || (param.IsNumeric() && f.NumericValue <= param.GetNumeric())))),
                "LT" => query.Where(x => x.CustomFields.Any(f => f.Type == type && f.Parent == null == !inner && f.Name == name && f.Namespace == nameSpace && ((param.IsDateTime() && f.DateValue < param.GetDate()) || (param.IsNumeric() && f.NumericValue < param.GetNumeric())))),
                _ => throw new EpcisException(ExceptionType.QueryParameterException, "Unknown Parameter")
            };
        }

        private static IQueryable<Event> ApplyMatchParameter(QueryParameter param, IQueryable<Event> query)
        {
            foreach (var value in param.Values)
            {
                query = query.Where(x => x.Epcs.Any(e => param.GetMatchEpcTypes().Contains(e.Type) && EF.Functions.Like(e.Id, value.Replace("*", "%"))));
            }

            return query;
        }
    }
}
