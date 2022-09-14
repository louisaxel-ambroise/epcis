using FasTnT.Application.Services;
using FasTnT.Application.Store;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Queries.Poll;

namespace FasTnT.Application.UseCases.ExecuteStandardQuery
{
    public class ExecuteStandardQueryHandler : IExecuteStandardQueryHandler
    {
        private readonly EpcisContext _context;
        private readonly IEnumerable<IStandardQuery> _standardQueries;

        public ExecuteStandardQueryHandler(EpcisContext context, IEnumerable<IStandardQuery> standardQueries)
        {
            _context = context;
            _standardQueries = standardQueries;
        }

        public Task<QueryResponse> ExecuteQueryAsync(string queryName, IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken)
        {
            var query = _standardQueries.SingleOrDefault(x => x.Name == queryName);

            if (query is null)
            {
                throw new EpcisException(ExceptionType.NoSuchNameException, $"Query '{queryName}' not found.");
            }

            return query.ExecuteAsync(_context, parameters, cancellationToken);
        }
    }
}
