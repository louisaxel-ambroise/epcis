using FasTnT.Domain.Model.CustomQueries;

namespace FasTnT.Application.UseCases.Queries;

public interface IGetQueryDetailsHandler
{
    Task<StoredQuery> GetQueryDetailsAsync(string name, CancellationToken cancellationToken);
}
