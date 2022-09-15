using FasTnT.Domain.Model.CustomQueries;

namespace FasTnT.Application.UseCases.CustomQueries
{
    public interface IGetCustomQueryDetailsHandler
    {
        Task<CustomQuery> GetCustomQueryDetailsAsync(string name, CancellationToken cancellationToken);
    }
}
