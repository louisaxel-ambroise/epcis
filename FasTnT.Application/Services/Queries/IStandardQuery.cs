using FasTnT.Application.Store;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Services.Queries;

public interface IStandardQuery
{
    public string Name { get; }
    public bool AllowSubscription { get; }

    Task<QueryResponse> ExecuteAsync(EpcisContext context, IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken);
}
