using FasTnT.Domain.Model;

namespace FasTnT.Application.UseCases.Captures;

public interface ICaptureRequest
{
    Task<Request> StoreAsync(Request request, CancellationToken cancellationToken);
}
