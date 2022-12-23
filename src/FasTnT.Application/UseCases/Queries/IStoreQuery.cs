using FasTnT.Domain.Model.CustomQueries;

namespace FasTnT.Application.UseCases.Queries;

public interface IStoreQuery
{
    Task<StoredQuery> StoreQueryAsync(StoredQuery query, CancellationToken cancellationToken);
}
