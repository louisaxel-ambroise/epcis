using FasTnT.Application.Store;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Services.Queries;

public class EpcisQueryContext
{
    private readonly IEpcisDataSource _query;
    private readonly IEnumerable<QueryParameter> _parameters;

    public EpcisQueryContext(IEpcisDataSource query, IEnumerable<QueryParameter> parameters)
    {
        _query = query;
        _parameters = parameters ?? Array.Empty<QueryParameter>();
    }

    public EpcisQueryContext MergeParameters(IEnumerable<QueryParameter> parameters) => new(_query, parameters.Union(_parameters));

    public Task<QueryData> ExecuteAsync(EpcisContext context, CancellationToken cancellationToken) => _query.ExecuteAsync(context, _parameters, cancellationToken);
}
