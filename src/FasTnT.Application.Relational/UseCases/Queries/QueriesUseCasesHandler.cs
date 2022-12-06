using FasTnT.Application.Services.Queries;
using FasTnT.Application.Services.Users;
using FasTnT.Application.UseCases.Queries;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.CustomQueries;
using FasTnT.Domain.Model.Queries;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Relational.UseCases.Queries;

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

    public async Task<IEnumerable<StoredQuery>> ListQueriesAsync(Pagination pagination, CancellationToken cancellationToken)
    {
        var queries = await _context.Set<StoredQuery>()
            .AsNoTracking()
            .Include(x => x.Parameters)
            .Skip(pagination.StartFrom).Take(pagination.PerPage)
            .ToListAsync(cancellationToken);

        return queries;
    }

    public async Task<StoredQuery> GetQueryDetailsAsync(string name, CancellationToken cancellationToken)
    {
        var query = await _context.Set<StoredQuery>()
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
        if (await _context.Set<StoredQuery>().AnyAsync(x => x.Name == query.Name, cancellationToken))
        {
            throw new EpcisException(ExceptionType.ValidationException, $"Query '{query.Name}' already exists.");
        }

        query.UserId = _currentUser.UserId;

        _context.Set<StoredQuery>().Add(query);
        await _context.SaveChangesAsync(cancellationToken);

        return query;
    }

    public async Task<StoredQuery> DeleteQueryAsync(string queryName, CancellationToken cancellationToken)
    {
        var query = await _context.Set<StoredQuery>()
               .Include(x => x.Subscriptions)
               .SingleOrDefaultAsync(x => x.Name == queryName, cancellationToken);

        if (query is null)
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Query '{queryName}' not found.");
        }
        if (query.UserId != _currentUser.UserId)
        {
            throw new EpcisException(ExceptionType.ValidationException, $"Query '{queryName}' was stored by another user.");
        }
        if (query.Subscriptions.Any())
        {
            throw new EpcisException(ExceptionType.ValidationException, $"Query '{queryName}' has active subscriptions.");
        }

        _context.Set<StoredQuery>().Remove(query);
        await _context.SaveChangesAsync(cancellationToken);

        return query;
    }

    public async Task<QueryResponse> ExecuteQueryAsync(string queryName, IEnumerable<QueryParameter> parameters, CancellationToken cancellationToken)
    {
        var query = await _context.Set<StoredQuery>()
            .AsNoTracking()
            .Include(x => x.Parameters)
            .SingleOrDefaultAsync(x => x.Name == queryName, cancellationToken);

        if (query is null)
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Query '{queryName}' not found.");
        }

        var performer = _queries.Single(x => x.Name == query.DataSource);
        var context = new EpcisQueryContext(performer, query.Parameters)
            .MergeParameters(parameters)
            .MergeParameters(_currentUser.DefaultQueryParameters);

        var response = await context.ExecuteAsync(cancellationToken);

        return new(queryName, response.EventList, response.VocabularyList);
    }
}
