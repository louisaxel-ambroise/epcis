using FasTnT.Application.Services.Users;
using FasTnT.Application.Store;
using FasTnT.Application.Validators;
using FasTnT.Domain.Commands.Capture;
using FasTnT.Domain.Infrastructure.Exceptions;
using MediatR;

namespace FasTnT.Application.Commands;

public class CaptureEpcisRequestCommandHandler : IRequestHandler<CaptureEpcisRequestCommand, CaptureEpcisRequestResponse>
{
    private readonly EpcisContext _context;
    private readonly ICurrentUser _currentUser;

    public CaptureEpcisRequestCommandHandler(EpcisContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<CaptureEpcisRequestResponse> Handle(CaptureEpcisRequestCommand request, CancellationToken cancellationToken)
    {
        if (!EpcisCaptureRequestValidator.IsValid(request.Request))
        {
            throw new EpcisException(ExceptionType.ValidationException, "EPCIS request is not valid");
        }

        request.Request.UserId = _currentUser.UserId;
        _context.Requests.Add(request.Request);

        await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return new(request.Request.Id.ToString());
    }
}
