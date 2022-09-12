using FasTnT.Application.Store;
using FasTnT.Domain.Queries.CustomQueries;
using FasTnT.Domain.Queries.Poll;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Queries.CustomQueries;

public record ListCustomQueryDefinition() : IRequest<IEpcisResponse>;

public class ListCustomQueryDefinitionHandler : IRequestHandler<ListCustomQueryDefinition, IEpcisResponse>
{
    private readonly EpcisContext _context;

    public ListCustomQueryDefinitionHandler(EpcisContext context)
    {
        _context = context;
    }

    public async Task<IEpcisResponse> Handle(ListCustomQueryDefinition request, CancellationToken cancellationToken)
    {
        var queries = await _context.CustomQueries
            .AsNoTracking()
            .Include(x => x.Parameters)
            .ToListAsync(cancellationToken);

        return new ListCustomQueriesResult(queries);
    }
}