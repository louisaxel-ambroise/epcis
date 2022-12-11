using FasTnT.Application.Database;
using FasTnT.Application.Services.Queries;
using FasTnT.Application.Services.Users;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.CustomQueries;
using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;
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
    private readonly IEnumerable<IEpcisDataSource> _dataSources;

    public QueriesUseCasesHandler(EpcisContext context, ICurrentUser currentUser, IEnumerable<IEpcisDataSource> dataSources)
    {
        _context = context;
        _currentUser = currentUser;
        _dataSources = dataSources;
    }

    public async Task<IEnumerable<StoredQuery>> ListQueriesAsync(Pagination pagination, CancellationToken cancellationToken)
    {
        var queries = await _context.Set<StoredQuery>()
            .AsNoTracking()
            .Skip(pagination.StartFrom).Take(pagination.PerPage)
            .ToListAsync(cancellationToken);

        return queries;
    }

    public async Task<StoredQuery> GetQueryDetailsAsync(string name, CancellationToken cancellationToken)
    {
        var query = await _context.Set<StoredQuery>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

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
               .FirstOrDefaultAsync(x => x.Name == queryName, cancellationToken);

        if (query is null)
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Query '{queryName}' not found.");
        }
        if (query.UserId != _currentUser.UserId)
        {
            throw new EpcisException(ExceptionType.ValidationException, $"Query '{queryName}' was stored by another user.");
        }
        if (await _context.Set<Subscription>().AnyAsync(x => x.QueryName == query.Name, cancellationToken))
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
            .FirstOrDefaultAsync(x => x.Name == queryName, cancellationToken);

        if (query is null)
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Query '{queryName}' not found.");
        }

        var performer = _dataSources.Single(x => x.Name == query.DataSource);
        var context = new EpcisQueryContext(performer, query.Parameters)
            .MergeParameters(parameters)
            .MergeParameters(_currentUser.DefaultQueryParameters);

        var response = await context.ExecuteAsync(cancellationToken);

        return new(queryName, response.EventList, response.VocabularyList);
    }
}
