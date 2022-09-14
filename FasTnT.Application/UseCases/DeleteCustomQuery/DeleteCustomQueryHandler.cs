using FasTnT.Application.Store;
using FasTnT.Domain.Infrastructure.Exceptions;

namespace FasTnT.Application.UseCases.DeleteCustomQuery;

public class DeleteCustomQueryHandler : IDeleteCustomQueryHandler
{
    private readonly EpcisContext _context;

    public DeleteCustomQueryHandler(EpcisContext context)
    {
        _context = context;
    }

    public Task DeleteQueryAsync(string queryName, CancellationToken cancellationToken)
    {
        var query = _context.CustomQueries.SingleOrDefault(x => x.Name == queryName);

        if (query is null)
        {
            throw new EpcisException(ExceptionType.NoSuchNameException, $"Query '{queryName}' not found.");
        }

        _context.CustomQueries.Remove(query);

        return _context.SaveChangesAsync(cancellationToken);
    }
}
