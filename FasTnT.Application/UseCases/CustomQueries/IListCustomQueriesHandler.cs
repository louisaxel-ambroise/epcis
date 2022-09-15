using FasTnT.Domain.Model.CustomQueries;

namespace FasTnT.Application.UseCases.CustomQueries
{
    public interface IListCustomQueriesHandler
    {
        Task<IEnumerable<CustomQuery>> ListCustomQueriesAsync(CancellationToken cancellationToken);
    }
}
