using FasTnT.Application.Store;
using FasTnT.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.UseCases.ListCaptureRequests
{
    internal class ListCaptureRequestsHandler : IListCaptureRequestsHandler
    {
        private readonly EpcisContext _context;

        public ListCaptureRequestsHandler(EpcisContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Request>> ListCapturesAsync(CancellationToken cancellationToken)
        {
            var captures = await _context.Requests
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToListAsync(cancellationToken);

            return captures;
        }
    }
}
