using FasTnT.Domain.Model.CustomQueries;

namespace FasTnT.Application.UseCases.GetCustomQueryDetails
{
    public interface IGetCustomQueryDetailsHandler
    {
        Task<CustomQuery> GetCustomQueryDetailsAsync(string name, CancellationToken cancellationToken);
    }
}
