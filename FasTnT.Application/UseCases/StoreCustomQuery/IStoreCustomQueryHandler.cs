using FasTnT.Domain.Model.CustomQueries;

namespace FasTnT.Application.UseCases.StoreCustomQuery;

public interface IStoreCustomQueryHandler
{
    Task<CustomQuery> StoreQueryAsync(CustomQuery query, CancellationToken cancellationToken);
}
