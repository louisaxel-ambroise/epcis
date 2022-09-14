using FasTnT.Domain.Model;

namespace FasTnT.Application.Services.Capture
{
    public interface IStoreEpcisDocumentHandler
    {
        Task<Request> StoreAsync(Request request, CancellationToken cancellationToken);
    }
}
