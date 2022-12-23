using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.UseCases.Captures;

public interface IListCaptureRequests
{
    Task<IEnumerable<Request>> ListCapturesAsync(Pagination pagination, CancellationToken cancellationToken);
}
