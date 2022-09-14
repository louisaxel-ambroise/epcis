using FasTnT.Application.Services;
using FasTnT.Application.Store;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Queries.Poll;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.UseCases.ExecuteCustomQuery
{
    public class ExecuteCustomQueryHandler : IExecuteCustomQueryHandler
    {
        private readonly EpcisContext _context;
        private readonly IEnumerable<IStandardQuery> _standardQueries;

        public ExecuteCustomQueryHandler(EpcisContext context, IEnumerable<IStandardQuery> standardQueries)
        {
            _context = context;
            _standardQueries = standardQueries;
        }

        public async Task<QueryResponse> ExecuteQueryAsync(string queryName, CancellationToken cancellationToken)
        {
            var standardQuery = _standardQueries.SingleOrDefault(x => x.Name == "SimpleEventQuery");
            var query = await _context.CustomQueries
                .AsNoTracking()
                .Include(x => x.Parameters)
                .SingleOrDefaultAsync(x => x.Name == queryName, cancellationToken);

            if (query is null)
            {
                throw new EpcisException(ExceptionType.NoSuchNameException, $"Query '{queryName}' not found.");
            }

            var response = await standardQuery.ExecuteAsync(_context, query.Parameters, cancellationToken);

            return response;
        }
    }
}
