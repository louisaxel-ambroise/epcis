using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.UseCases.TopLevelResources;

public interface IListBizLocations
{
    Task<IEnumerable<string>> ListBizLocations(Pagination pagination, CancellationToken cancellationToken);
}
