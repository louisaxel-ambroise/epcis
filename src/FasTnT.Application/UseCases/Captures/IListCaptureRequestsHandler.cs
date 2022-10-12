using FasTnT.Domain.Model;

namespace FasTnT.Application.UseCases.Captures;

public interface IListCaptureRequestsHandler
{
    Task<IEnumerable<Request>> ListCapturesAsync(CancellationToken cancellationToken);
}
