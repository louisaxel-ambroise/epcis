using FasTnT.Application.Database;
using FasTnT.Application.Services.Events;
using FasTnT.Application.Services.Notifications;
using FasTnT.Application.Services.Users;
using FasTnT.Application.Validators;
using FasTnT.Domain;
using FasTnT.Domain.Exceptions;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FasTnT.Application.Handlers;

public class CaptureHandler(EpcisContext context, ICurrentUser user, IEventNotifier notifier, IOptions<Constants> constants)
{
    public async Task<IEnumerable<Request>> ListCapturesAsync(Pagination pagination, CancellationToken cancellationToken)
    {
        var captures = await context
            .QueryEvents(user.DefaultQueryParameters)
            .Select(x => x.Request)
            .OrderBy(x => x.Id)
            .Skip(pagination.StartFrom)
            .Take(pagination.PerPage)
            .ToListAsync(cancellationToken);

        return captures;
    }

    public async Task<Request> GetCaptureDetailsAsync(string captureId, CancellationToken cancellationToken)
    {
        var capture = await context
            .QueryEvents(user.DefaultQueryParameters)
            .Select(x => x.Request)
            .FirstOrDefaultAsync(x => x.CaptureId == captureId, cancellationToken);

        return capture is null
            ? throw new EpcisException(ExceptionType.QueryParameterException, $"Capture not found: {captureId}")
            : capture;
    }

    public async Task<Request> StoreAsync(Request request, CancellationToken cancellationToken)
    {
        if (!RequestValidator.IsValid(request))
        {
            throw new EpcisException(ExceptionType.ValidationException, "EPCIS request is not valid");
        }
        if (request.Events.Count >= constants.Value.MaxEventsCapturePerCall)
        {
            throw new EpcisException(ExceptionType.CaptureLimitExceededException, "Capture Payload too large");
        }
        if (!HeaderValidator.IsValid(request.StandardBusinessHeader))
        {
            throw new EpcisException(ExceptionType.ValidationException, "Standard Business Header in EPCIS request is not valid");
        }

        request.UserId = user.UserId;
        request.Events.ForEach(evt => evt.EventId ??= EventHash.Compute(evt));

        using (var transaction = await context.Database.BeginTransactionAsync(cancellationToken))
        {
            context.Add(request);
            await context.SaveChangesAsync(cancellationToken);

            request.RecordTime = DateTime.UtcNow;
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        notifier.RequestCaptured(request);

        return request;
    }
}
