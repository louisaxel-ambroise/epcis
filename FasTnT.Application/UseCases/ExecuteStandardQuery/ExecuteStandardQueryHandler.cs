using FasTnT.Application.Services.Queries;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Store;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.UseCases.ExecuteStandardQuery
{
    public class ExecuteStandardQueryHandler : IExecuteStandardQueryHandler
    {
        private readonly EpcisContext _context;
        private readonly ICurrentUser _currentUser;
        private readonly IEnumerable<IStandardQuery> _standardQueries;

        public ExecuteStandardQueryHandler(EpcisContext context, ICurrentUser currentUser, IEnumerable<IStandardQuery> standardQueries)
        {
            _context = context;
            _currentUser = currentUser;
            _standardQueries = standardQueries;
        }

        public Task<QueryResponse> ExecuteQueryAsync(string queryName, IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken)
        {
            var query = _standardQueries.SingleOrDefault(x => x.Name == queryName);

            if (query is null)
            {
                throw new EpcisException(ExceptionType.NoSuchNameException, $"Query '{queryName}' not found.");
            }

            var applyParams = parameters.Union(_currentUser.DefaultQueryParameters);
            var response = query.ExecuteAsync(_context, applyParams, cancellationToken);

            return response;
        }
    }
}
