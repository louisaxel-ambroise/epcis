using FasTnT.Domain.Model.CustomQueries;

namespace FasTnT.Application.UseCases.DeleteCustomQuery;

public interface IDeleteCustomQueryHandler
{
    Task<CustomQuery> DeleteQueryAsync(string queryName, CancellationToken cancellationToken);
}
