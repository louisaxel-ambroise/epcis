using FasTnT.Domain.Model.CustomQueries;

namespace FasTnT.Application.UseCases.ListCustomQueries
{
    public interface IListCustomQueriesHandler
    {
        Task<IEnumerable<CustomQuery>> ListCustomQueriesAsync(CancellationToken cancellationToken);
    }
}
