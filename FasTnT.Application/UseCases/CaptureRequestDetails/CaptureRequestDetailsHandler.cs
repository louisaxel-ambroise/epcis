using FasTnT.Application.Store;
using FasTnT.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.UseCases.CaptureRequestDetails;

public class CaptureRequestDetailsHandler : ICaptureRequestDetailsHandler
{
    private readonly EpcisContext _context;

    public CaptureRequestDetailsHandler(EpcisContext context)
    {
        _context = context;
    }

    public Task<Request> GetCaptureDetails(int captureId, CancellationToken cancellationToken)
    {
        var query = _context.Requests
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == captureId, cancellationToken);

        return query;
    }
}
