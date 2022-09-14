using FasTnT.Domain.Model.CustomQueries;

namespace FasTnT.Application.UseCases.StoreCustomQuery;

public interface IStoreCustomQueryHandler
{
    Task StoreQueryAsync(CustomQuery query, CancellationToken cancellationToken);
}
