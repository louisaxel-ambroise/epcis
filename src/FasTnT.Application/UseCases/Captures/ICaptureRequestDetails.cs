using FasTnT.Domain.Model;

namespace FasTnT.Application.UseCases.Captures;

public interface ICaptureRequestDetails
{
    Task<Request> GetCaptureDetailsAsync(string captureId, CancellationToken cancellationToken);
}
