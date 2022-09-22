using FasTnT.Domain.Model;

namespace FasTnT.Application.UseCases.Captures
{
    public interface ICaptureRequestHandler
    {
        Task<Request> StoreAsync(Request request, CancellationToken cancellationToken);
    }
}
