using FasTnT.Application.Services.Queries;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Store;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.CustomQueries;
using FasTnT.Domain.Model.Queries;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.UseCases.CustomQueries;

public class CustomQueriesUseCasesHandler :
    IDeleteCustomQueryHandler,
    IExecuteCustomQueryHandler,
    IGetCustomQueryDetailsHandler,
    IListCustomQueriesHandler,
    IStoreCustomQueryHandler
{
    private readonly EpcisContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IEnumerable<IStandardQuery> _standardQueries;

    public CustomQueriesUseCasesHandler(EpcisContext context, ICurrentUser currentUser, IEnumerable<IStandardQuery> standardQueries)
    {
        _context = context;
        _currentUser = currentUser;
        _standardQueries = standardQueries;
    }

    public async Task<CustomQuery> DeleteQueryAsync(string queryName, CancellationToken cancellationToken)
    {
        var query = await _context.CustomQueries
            .SingleOrDefaultAsync(x => x.Name == queryName, cancellationToken);

        if (query is null)
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Query '{queryName}' not found.");
        }

        _context.CustomQueries.Remove(query);

        await _context.SaveChangesAsync(cancellationToken);

        return query;
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

    public async Task<CustomQuery> GetCustomQueryDetailsAsync(string name, CancellationToken cancellationToken)
    {
        var customQuery = await _context.CustomQueries
            .AsNoTracking()
            .Include(x => x.Parameters)
            .SingleOrDefaultAsync(x => x.Name == name, cancellationToken);

        if (customQuery is null)
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Query not found: '{name}'");
        }

        return customQuery;
    }

    public async Task<IEnumerable<CustomQuery>> ListCustomQueriesAsync(CancellationToken cancellationToken)
    {
        var queries = await _context.CustomQueries
            .AsNoTracking()
            .Include(x => x.Parameters)
            .ToListAsync(cancellationToken);

        return queries;
    }

    public async Task<CustomQuery> StoreQueryAsync(CustomQuery query, CancellationToken cancellationToken)
    {
        if (await _context.CustomQueries.AnyAsync(x => x.Name == query.Name, cancellationToken))
        {
            throw new EpcisException(ExceptionType.ValidationException, $"Query already exists: '{query.Name}'");
        }

        _context.CustomQueries.Add(query);

        await _context.SaveChangesAsync(cancellationToken);

        return query;
    }
}
