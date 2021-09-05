using FasTnT.Application.Services;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;
using FasTnT.Domain.Queries.Poll;
using FasTnT.Domain.Utils;
using FasTnT.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Application.Queries.Poll
{
    public class SimpleMasterDataQuery : IEpcisQuery
    {
        private readonly EpcisContext _context;
        private int? _maxEventCount;

        public SimpleMasterDataQuery(EpcisContext context)
        {
            _context = context;
        }

        public string Name => nameof(SimpleMasterDataQuery);
        public bool AllowSubscription => false;

        // TODO: apply parameters
        public async Task<PollResponse> HandleAsync(IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken)
        {
            var query = _context.MasterData.AsSplitQuery().AsNoTrackingWithIdentityResolution();

            foreach(var parameter in parameters)
            {
                query = ApplyParameter(parameter, query);
            }

            var result = await query.ToListAsync(cancellationToken);

            if (_maxEventCount.HasValue && result.Count > _maxEventCount)
            {
                throw new EpcisException(ExceptionType.QueryTooLargeException, $"Query returned more than the {_maxEventCount} elements allowed.");
            }

            return new(Name) { VocabularyList = result };
        }

        private IQueryable<MasterData> ApplyParameter(QueryParameter param, IQueryable<MasterData> query)
        {
            return param.Name switch
            {
                "maxElementCount" => ParseMaxElementCount(param, query),
                "vocabularyName" => query.Where(x => x.Type == param.Value()),
                "EQ_name" => query.Where(x => x.Id == param.Value()),
                "includeAttributes" => query.Include(x => x.Attributes).ThenInclude(x => x.Fields),
                "attributeNames" => query.Include(x => x.Attributes.Where(a => param.Values.Contains(a.Id))).ThenInclude(x => x.Fields),
                "includeChildren" => query,
                "HASATTR" => query.Where(x => x.Attributes.Any(a => a.Id == param.Value())),
                _ => throw new EpcisException(ExceptionType.QueryParameterException, $"Parameter is not implemented: {param.Name}")
            };
        }
        private IQueryable<MasterData> ParseMaxElementCount(QueryParameter param, IQueryable<MasterData> query)
        {
            _maxEventCount = param.GetIntValue();

            return query.Take(1 + _maxEventCount.Value);
        }
    }
}
