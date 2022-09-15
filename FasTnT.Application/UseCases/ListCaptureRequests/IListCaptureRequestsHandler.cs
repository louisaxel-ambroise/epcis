using FasTnT.Domain.Model;

namespace FasTnT.Application.UseCases.ListCaptureRequests
{
    public interface IListCaptureRequestsHandler
    {
        Task<IEnumerable<Request>> ListCapturesAsync(CancellationToken cancellationToken);
    }
}
