using FasTnT.Domain.Model.CustomQueries;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.UseCases.Queries;

public interface IListQueriesHandler
{
    Task<IEnumerable<StoredQuery>> ListQueriesAsync(Pagination pagination, CancellationToken cancellationToken);
}
