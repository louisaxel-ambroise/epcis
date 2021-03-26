using FasTnT.Infrastructure.Database;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Application.Commands
{
    public class CaptureEpcisRequestCommandHandler : ICommandHandler<CaptureEpcisRequestCommand, CaptureEpcisRequestResponse>
    {
        private readonly EpcisContext _context;

        public CaptureEpcisRequestCommandHandler(EpcisContext context)
        {
            _context = context;
        }

        public async Task<CaptureEpcisRequestResponse> Handle(CaptureEpcisRequestCommand request, CancellationToken cancellationToken)
        {
            _context.Requests.Add(request.Request);
            await _context.SaveChangesAsync(cancellationToken);

            return new CaptureEpcisRequestResponse
            {
                CommandId = request.Request.Id.ToString()
            };
        }
    }
}
