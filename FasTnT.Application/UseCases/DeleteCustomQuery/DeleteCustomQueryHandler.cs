using FasTnT.Application.Store;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.CustomQueries;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.UseCases.DeleteCustomQuery;

public class DeleteCustomQueryHandler : IDeleteCustomQueryHandler
{
    private readonly EpcisContext _context;

    public DeleteCustomQueryHandler(EpcisContext context)
    {
        _context = context;
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
}
