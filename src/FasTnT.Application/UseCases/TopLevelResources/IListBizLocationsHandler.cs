using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.UseCases.TopLevelResources;

public interface IListBizLocationsHandler
{
    Task<IEnumerable<string>> ListBizLocations(Pagination pagination, CancellationToken cancellationToken);
}
