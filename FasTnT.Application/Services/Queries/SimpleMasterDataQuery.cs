using FasTnT.Application.Services;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;
using FasTnT.Domain.Queries;
using FasTnT.Domain.Utils;
using FasTnT.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Queries;

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
            throw new EpcisException(ExceptionType.QueryTooLargeException, $"Query returned too many events.")
            {
                QueryName = Name
            }; ;
        }

        return new PollMasterdataResponse(Name, result);
    }

    private IQueryable<MasterData> ApplyParameter(QueryParameter param, IQueryable<MasterData> query)
    {
        return param.Name switch
        {
            "maxElementCount" => ParseMaxElementCount(param, query),
            "includeAttributes" => param.GetBoolValue() ? query.Include(x => x.Attributes).ThenInclude(x => x.Fields) : query,
            "includeChildren" => param.GetBoolValue() ? query.Include(x => x.Children) : query,
            "vocabularyName" => query.Where(x => x.Type == param.Value()),
            "EQ_name" => query.Where(x => param.Values.Any(v => v == x.Id)),
            "WD_name" => query.Where(x => param.Values.Any(v => v == x.Id) || x.Hierarchies.Any(h => param.Values.Any(v => v == h.ParentId))),
            "attributeNames" => query.Include(x => x.Attributes.Where(a => param.Values.Contains(a.Id))).ThenInclude(x => x.Fields),
            "HASATTR" => query.Where(x => x.Attributes.Any(a => a.Id == param.Value())),
                
            var s when s.StartsWith("EQATTR_") => ApplyEqAttrParameter(param, query),

            _ => throw new EpcisException(ExceptionType.QueryParameterException, $"Parameter is invalid for simplemasterdata query: {param.Name}")
        };
    }

    private IQueryable<MasterData> ParseMaxElementCount(QueryParameter param, IQueryable<MasterData> query)
    {
        _maxEventCount = param.GetIntValue();

        return query.Take(1 + _maxEventCount.Value);
    }

    private static IQueryable<MasterData> ApplyEqAttrParameter(QueryParameter param, IQueryable<MasterData> query)
    {
        var attributeName = param.Name["EQATTR_".Length..];

        return query.Where(x => x.Attributes.Any(x => x.Id == attributeName && param.Values.Any(v => v == x.Value)));
    }
}
