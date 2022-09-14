namespace FasTnT.Application.UseCases.DeleteCustomQuery;

public interface IDeleteCustomQueryHandler
{
    Task DeleteQueryAsync(string queryName, CancellationToken cancellationToken);
}
