using FasTnT.Application.Database;
using FasTnT.Application.Services.Events;
using FasTnT.Application.Services.Subscriptions;
using FasTnT.Application.Services.Users;
using FasTnT.Application.UseCases.DataSources.Utils;
using FasTnT.Application.Validators;
using FasTnT.Domain;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Queries;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.UseCases.Captures;

public class CaptureUseCases :
    IListCaptureRequests,
    ICaptureRequestDetails,
    ICaptureRequest
{
    private readonly EpcisContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly ISubscriptionListener _subscriptionListener;

    public CaptureUseCases(EpcisContext context, ICurrentUser currentUser, ISubscriptionListener subscriptionListener)
    {
        _context = context;
        _currentUser = currentUser;
        _subscriptionListener = subscriptionListener;
    }

    public async Task<IEnumerable<Request>> ListCapturesAsync(Pagination pagination, CancellationToken cancellationToken)
    {
        var captures = await _context
            .QueryEvents(_currentUser.DefaultQueryParameters)
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
            .QueryEvents(_currentUser.DefaultQueryParameters)
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

        var captureTime = DateTime.UtcNow;

        request.CaptureTime = captureTime;
        request.UserId = _currentUser.UserId;
        request.Events.ForEach(evt =>
        {
            evt.CaptureTime = captureTime;

            if (string.IsNullOrEmpty(evt.EventId))
            {
                evt.EventId = EventHash.Compute(evt);
            }
        });

        _context.Add(request);

        await _context.SaveChangesAsync(cancellationToken);
        await _subscriptionListener.TriggerAsync(new[] { "stream" }, cancellationToken);

        return request;
    }
}
