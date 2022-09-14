using FasTnT.Domain.Model;

namespace FasTnT.Application.UseCases.CaptureRequestDetails;

public interface ICaptureRequestDetailsHandler
{
    Task<Request> GetCaptureDetails(int captureId, CancellationToken cancellationToken);
}
