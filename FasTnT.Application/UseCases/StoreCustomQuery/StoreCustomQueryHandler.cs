using FasTnT.Application.Store;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model.CustomQueries;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.UseCases.StoreCustomQuery;

public class StoreCustomQueryHandler : IStoreCustomQueryHandler
{
    private readonly EpcisContext _context;

    public StoreCustomQueryHandler(EpcisContext context)
    {
        _context = context;
    }

    public async Task StoreQueryAsync(CustomQuery query, CancellationToken cancellationToken)
    {
        if(await _context.CustomQueries.AnyAsync(x => x.Name == query.Name, cancellationToken))
        {
            throw new EpcisException(ExceptionType.ValidationException, $"Query already exists: '{query.Name}'");
        };

        _context.CustomQueries.Add(query);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
