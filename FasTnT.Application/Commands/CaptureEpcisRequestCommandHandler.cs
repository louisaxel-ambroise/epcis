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

        public CaptureEpcisRequestCommandHandler(EpcisContext context) => _context = context;

        public async Task<CaptureEpcisRequestResponse> Handle(CaptureEpcisRequestCommand request, CancellationToken cancellationToken)
        {
            _context.Requests.Add(request.Request);

            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return new(request.Request.Id.ToString());
        }
    }
}
