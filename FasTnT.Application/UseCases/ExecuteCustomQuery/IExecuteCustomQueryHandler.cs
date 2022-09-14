using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.UseCases.ExecuteCustomQuery
{
    public interface IExecuteCustomQueryHandler
    {
        Task<QueryResponse> ExecuteQueryAsync(string queryName, CancellationToken cancellationToken);
    }
}
