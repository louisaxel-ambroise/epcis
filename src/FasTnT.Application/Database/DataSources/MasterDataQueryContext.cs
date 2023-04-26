using FasTnT.Application.Database.DataSources.Utils;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Masterdata;
using FasTnT.Domain.Model.Queries;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FasTnT.Application.Database.DataSources;

internal class MasterDataQueryContext
{
    private int _take = int.MaxValue;
    private readonly List<Func<IQueryable<MasterData>, IQueryable<MasterData>>> _filters = new();
    private readonly List<string> _attributeNames = new();
    private bool _includeAttributes, _includeChildren;
    private readonly EpcisContext _context;

    internal MasterDataQueryContext(EpcisContext context, IEnumerable<QueryParameter> parameters)
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
            case "includeAttributes":
                _includeAttributes = param.AsBool(); break;
            case "includeChildren":
                _includeChildren = param.AsBool(); break;
            case "attributeNames":
                _attributeNames.AddRange(param.Values); break;
            // Family filters
            case var s when s.StartsWith("EQATTR_"):
                ApplyEqAttrParameter(param); break;
            default:    
                throw new EpcisException(ExceptionType.QueryParameterException, $"Parameter is invalid for simplemasterdata query: {param.Name}");
        }
    }

    public IQueryable<MasterData> ApplyTo(IQueryable<MasterData> query)
    {
        var masterdata = _filters
            .Aggregate(query, (q, f) => f(q))
            .OrderBy(x => x.Id)
            .Take(_take);

        if (_includeChildren)
        {
            masterdata = masterdata.Include(x => x.Children);
        }
        if (_includeAttributes)
        {
            masterdata = masterdata.Include(x => x.Attributes.Where(x => _attributeNames.Count == 0 || _attributeNames.Contains(x.Id)));
        }

        return masterdata;
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