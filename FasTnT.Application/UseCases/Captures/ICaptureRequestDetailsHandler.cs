using FasTnT.Domain.Model;

namespace FasTnT.Application.UseCases.Captures;

public interface ICaptureRequestDetailsHandler
{
    Task<Request> GetCaptureDetailsAsync(int captureId, CancellationToken cancellationToken);
}
