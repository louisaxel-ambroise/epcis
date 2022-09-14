using FasTnT.Domain.Queries.Poll;

namespace FasTnT.Application.UseCases.ExecuteStandardQuery
{
    public interface IExecuteStandardQueryHandler
    {
        Task<QueryResponse> ExecuteQueryAsync(string queryName, IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken);
    }
}
