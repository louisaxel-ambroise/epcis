using FasTnT.Application.Database;
using FasTnT.Application.DataSources.Utils;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Masterdata;
using FasTnT.Domain.Model.Queries;
using System.Linq.Expressions;

namespace FasTnT.Application.DataSources;

public class MasterDataQueryContext
{
    private int _take = int.MaxValue;
    private readonly List<Func<IQueryable<MasterData>, IQueryable<MasterData>>> _filters = new();
    private readonly EpcisContext _context;

    public MasterDataQueryContext(EpcisContext context, IEnumerable<QueryParameter> parameters)
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
            // Simple filters
            case "maxElementCount":
                _take = Math.Min(_take, param.AsInt()); break;
            case "vocabularyName":
                Filter(x => x.Type == param.AsString()); break;
            case "EQ_userID":
                Filter(x => param.Values.Contains(x.Request.UserId)); break;
            case "EQ_name":
                Filter(x => param.Values.Any(v => v == x.Id)); break;
            case "WD_name":
                Filter(x => _context.Set<MasterDataHierarchy>().Any(h => h.Type == x.Type && h.Root == x.Id && param.Values.Contains(h.Id))); break;
            case "HASATTR":
                Filter(x => x.Attributes.Any(a => a.Id == param.AsString())); break;
            // Family filters
            case var s when s.StartsWith("EQATTR_"):
                ApplyEqAttrParameter(param); break;
            case "attributeNames" or "includeAttributes" or "includeChildren": break; // EF Core automatically included these field due to the "OwnsMany" mapping. Maybe review it to only display them when the parameters are specified?
            // Any other case is an unknown parameter and should raise a QueryParameter Exception
            default:
                throw new EpcisException(ExceptionType.QueryParameterException, $"Parameter is invalid for simplemasterdata query: {param.Name}");
        }
    }

    public IQueryable<MasterData> ApplyTo(IQueryable<MasterData> query)
    {
        return _filters
            .Aggregate(query, (q, f) => f(q))
            .OrderBy(x => x.Id)
            .Take(_take);
    }

    private void ApplyEqAttrParameter(QueryParameter param)
    {
        var attributeName = param.Name["EQATTR_".Length..];

        Filter(x => x.Attributes.Any(x => x.Id == attributeName && param.Values.Any(v => v == x.Value)));
    }

    private void Filter(Expression<Func<MasterData, bool>> expression)
    {
        _filters.Add(evt => evt.Where(expression));
    }
}