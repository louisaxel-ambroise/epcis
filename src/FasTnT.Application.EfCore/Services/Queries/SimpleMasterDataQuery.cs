using FasTnT.Application.EfCore.Store;
using FasTnT.Application.Services.Queries;
using FasTnT.Application.Services.Queries.Utils;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Queries;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.EfCore.Services.Queries;

public class SimpleMasterDataQuery : IEpcisDataSource
{
    private int? _maxEventCount;
    private readonly EpcisContext _context;

    public string Name => nameof(SimpleMasterDataQuery);
    public bool AllowSubscription => false;

    public SimpleMasterDataQuery(EpcisContext context)
    {
        _context = context;
    }

    public async Task<QueryData> ExecuteAsync(IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken)
    {
        var query = _context.MasterData.AsSplitQuery().AsNoTrackingWithIdentityResolution();

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
            var result = await query
                .Take(_maxEventCount ?? int.MaxValue)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            if (_maxEventCount.HasValue && result.Count > _maxEventCount)
            {
                throw new EpcisException(ExceptionType.QueryTooLargeException, $"Query returned too many events.")
                {
                    QueryName = Name
                };
            }

            return result;
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

    private IQueryable<MasterData> ApplyParameter(QueryParameter param, IQueryable<MasterData> query)
    {
        return param.Name switch
        {
            "maxElementCount" => ParseLimitEventCount(param, query, ref _maxEventCount),
            "includeAttributes" => param.GetBoolValue() ? query.Include(x => x.Attributes).ThenInclude(x => x.Fields) : query,
            "includeChildren" => param.GetBoolValue() ? query.Include(x => x.Children) : query,
            "vocabularyName" => query.Where(x => x.Type == param.Value()),
            "EQ_userID" => query.Where(x => param.Values.Contains(x.Request.UserId)),
            "EQ_name" => query.Where(x => param.Values.Any(v => v == x.Id)),
            "WD_name" => query.Where(x => param.Values.Any(v => v == x.Id) || x.Hierarchies.Any(h => param.Values.Any(v => v == h.ParentId))),
            "attributeNames" => query.Include(x => x.Attributes.Where(a => param.Values.Contains(a.Id))).ThenInclude(x => x.Fields),
            "HASATTR" => query.Where(x => x.Attributes.Any(a => a.Id == param.Value())),

            var s when s.StartsWith("EQATTR_") => ApplyEqAttrParameter(param, query),

            _ => throw new EpcisException(ExceptionType.QueryParameterException, $"Parameter is invalid for simplemasterdata query: {param.Name}")
        };
    }

    private static IQueryable<MasterData> ParseLimitEventCount(QueryParameter param, IQueryable<MasterData> query, ref int? destination)
    {
        destination = param.GetIntValue();

        return query;
    }

    private static IQueryable<MasterData> ApplyEqAttrParameter(QueryParameter param, IQueryable<MasterData> query)
    {
        var attributeName = param.Name["EQATTR_".Length..];

        return query.Where(x => x.Attributes.Any(x => x.Id == attributeName && param.Values.Any(v => v == x.Value)));
    }
}
