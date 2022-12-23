using FasTnT.Domain.Model.CustomQueries;

namespace FasTnT.Application.UseCases.Queries;

public interface IDeleteQuery
{
    Task<StoredQuery> DeleteQueryAsync(string queryName, CancellationToken cancellationToken);
}
