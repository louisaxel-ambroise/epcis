using FasTnT.Application.Database;
using FasTnT.Application.Services.Users;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.CustomQueries;
using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.UseCases.Queries;

public class QueriesUseCasesHandler :
    IListQueriesHandler,
    IGetQueryDetailsHandler,
    IStoreQueryHandler,
    IDeleteQueryHandler
{
    private readonly EpcisContext _context;
    private readonly ICurrentUser _currentUser;

    public QueriesUseCasesHandler(EpcisContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<StoredQuery>> ListQueriesAsync(Pagination pagination, CancellationToken cancellationToken)
    {
        var queries = await _context.Set<StoredQuery>()
            .AsNoTracking()
            .OrderBy(x => x.Id)
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

        _context.Add(query);
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

        _context.Remove(query);
        await _context.SaveChangesAsync(cancellationToken);

        return query;
    }
}
