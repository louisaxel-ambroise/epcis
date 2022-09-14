using FasTnT.Application.Store;
using FasTnT.Domain.Queries.Poll;

namespace FasTnT.Application.Services;

public interface IStandardQuery
{
    public string Name { get; }
    public bool AllowSubscription { get; }

    Task<QueryResponse> ExecuteAsync(EpcisContext context, IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken);
}
