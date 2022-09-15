using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.UseCases.CustomQueries
{
    public interface IExecuteCustomQueryHandler
    {
        Task<QueryResponse> ExecuteQueryAsync(string queryName, CancellationToken cancellationToken);
    }
}
