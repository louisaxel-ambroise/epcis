using FasTnT.Domain.Model.CustomQueries;

namespace FasTnT.Application.UseCases.CustomQueries;

public interface IStoreCustomQueryHandler
{
    Task<CustomQuery> StoreQueryAsync(CustomQuery query, CancellationToken cancellationToken);
}
