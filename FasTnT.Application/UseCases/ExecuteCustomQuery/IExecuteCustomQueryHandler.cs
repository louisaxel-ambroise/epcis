using FasTnT.Domain.Queries.Poll;

namespace FasTnT.Application.UseCases.ExecuteCustomQuery
{
    public interface IExecuteCustomQueryHandler
    {
        Task<QueryResponse> ExecuteQueryAsync(string queryName, CancellationToken cancellationToken);
    }
}
