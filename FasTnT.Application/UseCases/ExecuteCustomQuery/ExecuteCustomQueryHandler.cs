using FasTnT.Application.Services.Queries;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Store;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Queries;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.UseCases.ExecuteCustomQuery
{
    public class ExecuteCustomQueryHandler : IExecuteCustomQueryHandler
    {
        private readonly EpcisContext _context;
        private readonly ICurrentUser _currentUser;
        private readonly IEnumerable<IStandardQuery> _standardQueries;

        public ExecuteCustomQueryHandler(EpcisContext context, ICurrentUser currentUser, IEnumerable<IStandardQuery> standardQueries)
        {
            _context = context;
            _currentUser = currentUser;
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

            var applyParams = query.Parameters.Union(_currentUser.DefaultQueryParameters);
            var response = await standardQuery.ExecuteAsync(_context, applyParams, cancellationToken);

            return response;
        }
    }
}
