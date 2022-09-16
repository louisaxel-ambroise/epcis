using FasTnT.Application.Store;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Services.Queries;

public interface IEpcisDataSource
{
    public string Name { get; }
    public bool AllowSubscription { get; }

    Task<QueryData> ExecuteAsync(EpcisContext context, IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken);
}
