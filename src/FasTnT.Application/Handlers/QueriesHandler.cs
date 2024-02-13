using FasTnT.Application.Database;
using FasTnT.Application.Services.Users;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model.Queries;
using FasTnT.Domain.Model.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Handlers;

public class QueriesHandler(EpcisContext context, ICurrentUser user)
{
    public async Task<IEnumerable<StoredQuery>> ListQueriesAsync(Pagination pagination, CancellationToken cancellationToken)
    {
        var queries = await context.Set<StoredQuery>()
            .OrderBy(x => x.Id)
            .Skip(pagination.StartFrom).Take(pagination.PerPage)
            .ToListAsync(cancellationToken);

        return queries;
    }

    public async Task<StoredQuery> GetQueryDetailsAsync(string name, CancellationToken cancellationToken)
    {
        var query = await context.Set<StoredQuery>()
            .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

        if (query is null)
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Query '{name}' not found.");
        }

        return query;
    }

    public async Task<StoredQuery> StoreQueryAsync(StoredQuery query, CancellationToken cancellationToken)
    {
        if (await context.Set<StoredQuery>().AnyAsync(x => x.Name == query.Name, cancellationToken))
        {
            throw new EpcisException(ExceptionType.ValidationException, $"Query '{query.Name}' already exists.");
        }

        query.UserId = user.UserId;

        context.Add(query);
        await context.SaveChangesAsync(cancellationToken);

        return query;
    }

    public async Task<StoredQuery> DeleteQueryAsync(string queryName, CancellationToken cancellationToken)
    {
        var query = await context.Set<StoredQuery>()
               .FirstOrDefaultAsync(x => x.Name == queryName, cancellationToken);

        if (query is null)
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Query '{queryName}' not found.");
        }
        if (query.UserId != user.UserId)
        {
            throw new EpcisException(ExceptionType.ValidationException, $"Query '{queryName}' was stored by another user.");
        }
        if (await context.Set<Subscription>().AnyAsync(x => x.QueryName == query.Name, cancellationToken))
        {
            throw new EpcisException(ExceptionType.ValidationException, $"Query '{queryName}' has active subscriptions.");
        }

        context.Remove(query);
        await context.SaveChangesAsync(cancellationToken);

        return query;
    }
}
