using FasTnT.Application.Database;
using FasTnT.Application.Services.Events;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Validators;
using FasTnT.Domain;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Queries;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.Handlers;

public class CaptureHandler
{
    private readonly EpcisContext _context;
    private readonly ICurrentUser _user;

    public CaptureHandler(EpcisContext context, ICurrentUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<IEnumerable<Request>> ListCapturesAsync(Pagination pagination, CancellationToken cancellationToken)
    {
        var captures = await _context
            .QueryEvents(_user.DefaultQueryParameters)
            .Select(x => x.Request)
            .OrderBy(x => x.Id)
            .Skip(pagination.StartFrom)
            .Take(pagination.PerPage)
            .ToListAsync(cancellationToken);

        return captures;
    }

    public async Task<Request> GetCaptureDetailsAsync(string captureId, CancellationToken cancellationToken)
    {
        var capture = await _context
            .QueryEvents(_user.DefaultQueryParameters)
            .Select(x => x.Request)
            .FirstOrDefaultAsync(x => x.CaptureId == captureId, cancellationToken);

        if (capture is null)
        {
            throw new EpcisException(ExceptionType.QueryParameterException, $"Capture not found: {captureId}");
        }

        return capture;
    }

    public async Task<Request> StoreAsync(Request request, CancellationToken cancellationToken)
    {
        if (!RequestValidator.IsValid(request))
        {
            throw new EpcisException(ExceptionType.ValidationException, "EPCIS request is not valid");
        }
        if (request.Events.Count >= Constants.Instance.MaxEventsCapturePerCall)
        {
            throw new EpcisException(ExceptionType.CaptureLimitExceededException, "Capture Payload too large");
        }
        if (!HeaderValidator.IsValid(request.StandardBusinessHeader))
        {
            throw new EpcisException(ExceptionType.ValidationException, "Standard Business Header in EPCIS request is not valid");
        }

        request.UserId = _user.UserId;
        request.Events.ForEach(evt => evt.EventId ??= EventHash.Compute(evt));

        await _context.ExecuteTransactionAsync((token) => StoreRequestAsync(request, token), cancellationToken);
        EpcisEvents.RequestCaptured(request);

        return request;
    }

    private async Task StoreRequestAsync(Request request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(_context.Database.CurrentTransaction);
        
        _context.Add(request);
        await _context.SaveChangesAsync(cancellationToken);

        request.RecordTime = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
