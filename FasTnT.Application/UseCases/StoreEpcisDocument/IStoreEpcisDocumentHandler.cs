using FasTnT.Domain.Model;

namespace FasTnT.Application.UseCases.StoreEpcisDocument
{
    public interface IStoreEpcisDocumentHandler
    {
        Task<Request> StoreAsync(Request request, CancellationToken cancellationToken);
    }
}
