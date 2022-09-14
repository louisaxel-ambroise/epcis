using FasTnT.Application.Store;
using FasTnT.Domain.Model.CustomQueries;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.UseCases.ListCustomQueries
{
    internal class ListCustomQueriesHandler : IListCustomQueriesHandler
    {
        private readonly EpcisContext _context;

        public ListCustomQueriesHandler(EpcisContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomQuery>> ListCustomQueriesAsync(CancellationToken cancellationToken)
        {
            var queries = await _context.CustomQueries
                .AsNoTracking()
                .Include(x => x.Parameters)
                .ToListAsync(cancellationToken);

            return queries;
        }
    }
}
