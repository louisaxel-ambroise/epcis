﻿using FasTnT.Application.Services.Users;
using FasTnT.Application.Store;
using FasTnT.Application.Validators;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace FasTnT.Application.UseCases.Captures;

public class CaptureUseCasesHandler :
    IListCaptureRequestsHandler,
    ICaptureRequestDetailsHandler,
    ICaptureRequestHandler
{
    private readonly EpcisContext _context;
    private readonly ICurrentUser _currentUser;

    public CaptureUseCasesHandler(EpcisContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<Request>> ListCapturesAsync(CancellationToken cancellationToken)
    {
        var captures = await _context.Requests
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);

        return captures;
    }

    public async Task<Request> GetCaptureDetailsAsync(int captureId, CancellationToken cancellationToken)
    {
        var capture = await _context.Requests
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == captureId, cancellationToken);

        if (capture is null)
        {
            throw new EpcisException(ExceptionType.QueryParameterException, $"Capture not found: {captureId}");
        }

        return capture;
    }

    public async Task<Request> StoreAsync(Request request, CancellationToken cancellationToken)
    {
        if (!EpcisCaptureRequestValidator.IsValid(request))
        {
            throw new EpcisException(ExceptionType.ValidationException, "EPCIS request is not valid");
        }

        request.UserId = _currentUser.UserId;
        _context.Requests.Add(request);

        await _context.SaveChangesAsync(cancellationToken);

        return request;
    }
}
