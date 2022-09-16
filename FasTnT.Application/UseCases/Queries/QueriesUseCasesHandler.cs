using FasTnT.Application.Services.Queries;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Store;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.CustomQueries;
using FasTnT.Domain.Model.Queries;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.UseCases.Queries;

public class QueriesUseCasesHandler : 
    IListQueriesHandler,
    IGetQueryDetailsHandler,
    IStoreQueryHandler,
    IDeleteQueryHandler,
    IExecuteQueryHandler
{
    private readonly EpcisContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IEnumerable<IEpcisDataSource> _queries;

    public QueriesUseCasesHandler(EpcisContext context, ICurrentUser currentUser, IEnumerable<IEpcisDataSource> queries)
    {
        _context = context;
        _currentUser = currentUser;
        _queries = queries;
    }

    public async Task<IEnumerable<StoredQuery>> ListQueriesAsync(CancellationToken cancellationToken)
    {
        var queries = await _context.Queries
            .AsNoTracking()
            .Include(x => x.Parameters)
            .ToListAsync(cancellationToken);

        return queries;
    }

    public async Task<StoredQuery> GetQueryDetailsAsync(string name, CancellationToken cancellationToken)
    {
        var query = await _context.Queries
            .AsNoTracking()
            .Include(x => x.Parameters)
            .SingleOrDefaultAsync(x => x.Name == name, cancellationToken);

        if (query is null)
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Query '{name}' not found.");
        }

        return query;
    }

    public async Task<StoredQuery> StoreQueryAsync(StoredQuery query, CancellationToken cancellationToken)
    {
        if (await _context.Queries.AnyAsync(x => x.Name == query.Name, cancellationToken))
        {
            throw new EpcisException(ExceptionType.ValidationException, $"Query '{query.Name}' already exists.");
        }

        query.Username = _currentUser.Username;

        _context.Queries.Add(query);
        await _context.SaveChangesAsync(cancellationToken);

        return query;
    }

    public async Task<StoredQuery> DeleteQueryAsync(string queryName, CancellationToken cancellationToken)
    {
        var query = await _context.Queries
               .SingleOrDefaultAsync(x => x.Name == queryName, cancellationToken);

        if(query is null)
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Query '{queryName}' not found.");
        }
        if(query.Username != _currentUser.Username)
        {
            throw new EpcisException(ExceptionType.ValidationException, $"Query '{queryName}' was stored by another user.");
        }

        _context.Queries.Remove(query);
        await _context.SaveChangesAsync(cancellationToken);

        return query;
    }

    public async Task<QueryResponse> ExecuteQueryAsync(string queryName, IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken)
    {
        var query = await _context.Queries
            .AsNoTracking()
            .Include(x => x.Parameters)
            .SingleOrDefaultAsync(x => x.Name == queryName, cancellationToken);

        if (query is null)
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Query '{queryName}' not found.");
        }

        var performer = _queries.SingleOrDefault(x => x.Name == query.DataSource);
        var context = new EpcisQueryContext(performer, query.Parameters)
            .MergeParameters(parameters)
            .MergeParameters(_currentUser.DefaultQueryParameters);

        var response = await context.ExecuteAsync(_context, cancellationToken);

        return new (queryName, response.EventList, response.VocabularyList);
    }
}
