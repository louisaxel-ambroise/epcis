using FasTnT.Domain.Model.CustomQueries;

namespace FasTnT.Application.UseCases.Queries;

public interface IGetQueryDetails
{
    Task<StoredQuery> GetQueryDetailsAsync(string name, CancellationToken cancellationToken);
}
