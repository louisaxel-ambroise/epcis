using FasTnT.Application.Services.Queries;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Store;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.UseCases.StandardQueries;

public class StandardQueriesUseCasesHandler : 
    IGetStandardQueryNamesHandler,
    IExecuteStandardQueryHandler
{
    private readonly EpcisContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IEnumerable<IStandardQuery> _queries;

    public StandardQueriesUseCasesHandler(EpcisContext context, ICurrentUser currentUser, IEnumerable<IStandardQuery> queries)
    {
        _context = context;
        _currentUser = currentUser;
        _queries = queries;
    }

    public Task<QueryResponse> ExecuteQueryAsync(string queryName, IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken)
    {
        var query = _queries.SingleOrDefault(x => x.Name == queryName);

        if (query is null)
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Query '{queryName}' not found.");
        }

        var applyParams = parameters.Union(_currentUser.DefaultQueryParameters);
        var response = query.ExecuteAsync(_context, applyParams, cancellationToken);

        return response;
    }

    public Task<IEnumerable<string>> GetQueryNamesAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_queries.Select(x => x.Name));
    }
}
