using FasTnT.Application.Services.Users;
using FasTnT.Domain.Commands.Capture;
using FasTnT.Infrastructure.Database;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Application.Commands
{
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
            request.Request.UserId = _currentUser.UserId;
            _context.Requests.Add(request.Request);

            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new(request.Request.Id.ToString());
        }
    }
}
