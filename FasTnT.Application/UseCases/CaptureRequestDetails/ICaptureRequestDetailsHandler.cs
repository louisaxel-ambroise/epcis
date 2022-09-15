using FasTnT.Domain.Model;

namespace FasTnT.Application.UseCases.CaptureRequestDetails;

public interface ICaptureRequestDetailsHandler
{
    Task<Request> GetCaptureDetailsAsync(int captureId, CancellationToken cancellationToken);
}
