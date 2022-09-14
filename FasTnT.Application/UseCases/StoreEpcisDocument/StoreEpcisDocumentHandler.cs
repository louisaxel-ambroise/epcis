using FasTnT.Application.Services.Users;
using FasTnT.Application.Store;
using FasTnT.Application.Validators;
using FasTnT.Domain.Infrastructure.Exceptions;
using FasTnT.Domain.Model;

namespace FasTnT.Application.UseCases.StoreEpcisDocument
{
    public class StoreEpcisDocumentHandler : IStoreEpcisDocumentHandler
    {
        private readonly EpcisContext _context;
        private readonly ICurrentUser _currentUser;

        public StoreEpcisDocumentHandler(EpcisContext context, ICurrentUser currentUser)
        {
            _context = context;
            _currentUser = currentUser;
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
}
