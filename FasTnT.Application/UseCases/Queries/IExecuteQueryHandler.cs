using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.UseCases.Queries
{
    public interface IExecuteQueryHandler
    {
        Task<QueryResponse> ExecuteQueryAsync(string queryName, IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken);
    }
}
