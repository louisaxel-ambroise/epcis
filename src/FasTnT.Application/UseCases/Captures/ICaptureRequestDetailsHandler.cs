using FasTnT.Domain.Model;

namespace FasTnT.Application.UseCases.Captures;

public interface ICaptureRequestDetailsHandler
{
    Task<Request> GetCaptureDetailsAsync(string captureId, CancellationToken cancellationToken);
}
