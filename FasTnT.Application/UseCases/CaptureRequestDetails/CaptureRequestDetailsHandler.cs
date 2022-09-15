using FasTnT.Application.Store;
using FasTnT.Domain.Infrastructure.Exceptions;
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

    public async Task<Request> GetCaptureDetailsAsync(int captureId, CancellationToken cancellationToken)
    {
        var capture = await _context.Requests
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == captureId, cancellationToken);

        if(capture is null)
        {
            throw new EpcisException(ExceptionType.QueryParameterException, $"Capture not found: {captureId}");
        }

        return capture;
    }
}
