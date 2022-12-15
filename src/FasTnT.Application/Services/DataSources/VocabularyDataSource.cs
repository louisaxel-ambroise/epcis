using FasTnT.Application.Database;
using FasTnT.Application.Services.DataSources.Utils;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Masterdata;
using FasTnT.Domain.Model.Queries;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FasTnT.Application.Services.DataSources;

public class VocabularyDataSource : IEpcisDataSource
{
    private int? _maxEventCount;
    private readonly EpcisContext _context;

    public IQueryable<MasterData> Query { get; private set; }

    public VocabularyDataSource(EpcisContext context)
    {
        _context = context;
        Query = _context.Set<MasterData>().AsNoTracking();
    }

    public async Task<QueryData> ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            var result = await Query
                .Take(_maxEventCount ?? int.MaxValue)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            if (_maxEventCount.HasValue && result.Count > _maxEventCount)
            {
                throw new EpcisException(ExceptionType.QueryTooLargeException, $"Query returned too many events.");
            }

            return result;
        }
        catch
        {
            throw new EpcisException(ExceptionType.ImplementationException, "Query took too long to execute");
        }
    }

    public void Apply(QueryParameter param)
    {
        switch(param.Name)
        {
            // Simple filters
            case "maxElementCount": 
                ParseLimitEventCount(param); break;
            case "includeAttributes" or  "includeChildren": break; // TODO: Already included by the "OwnsMany". Maybe review it?
            case "vocabularyName": 
                Filter(x => x.Type == param.AsString()); break;
            case "EQ_userID": 
                Filter(x => param.Values.Contains(x.Request.UserId)); break;
            case "EQ_name": 
                Filter(x => param.Values.Any(v => v == x.Id)); break;
            case "WD_name": 
                Filter(x => _context.Set<MasterDataHierarchy>().Any(h => h.Type == x.Type && h.Root == x.Id && param.Values.Contains(x.Id))); break;
            case "attributeNames": 
                Query.Include(x => x.Attributes.Where(a => param.Values.Contains(a.Id))).ThenInclude(x => x.Fields); break;
            case "HASATTR": 
                Filter(x => x.Attributes.Any(a => a.Id == param.AsString())); break;
            // Family filters
            case var s when s.StartsWith("EQATTR_"): 
                ApplyEqAttrParameter(param); break;
            // Any other case is an unknown parameter and should raise a QueryParameter Exception
            default: 
                throw new EpcisException(ExceptionType.QueryParameterException, $"Parameter is invalid for simplemasterdata query: {param.Name}");
        }
    }

    private void ParseLimitEventCount(QueryParameter param)
    {
        _maxEventCount = param.AsInt();
    }

    private void ApplyEqAttrParameter(QueryParameter param)
    {
        var attributeName = param.Name["EQATTR_".Length..];

        Filter(x => x.Attributes.Any(x => x.Id == attributeName && param.Values.Any(v => v == x.Value)));
    }

    private void Filter(Expression<Func<MasterData, bool>> expression)
    {
        Query = Query.Where(expression);
    }
}
