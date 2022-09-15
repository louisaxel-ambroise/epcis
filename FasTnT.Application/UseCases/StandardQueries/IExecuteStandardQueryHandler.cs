using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.UseCases.StandardQueries
{
    public interface IExecuteStandardQueryHandler
    {
        Task<QueryResponse> ExecuteQueryAsync(string queryName, IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken);
    }
}
