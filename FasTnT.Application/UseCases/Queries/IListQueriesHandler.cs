using FasTnT.Domain.Model.CustomQueries;

namespace FasTnT.Application.UseCases.Queries;

public interface IListQueriesHandler
{
    Task<IEnumerable<StoredQuery>> ListQueriesAsync(CancellationToken cancellationToken);
}
