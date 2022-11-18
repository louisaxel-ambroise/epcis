using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.UseCases.TopLevelResources;

public interface IListReadPointsHandler
{
    Task<IEnumerable<string>> ListReadPoints(Pagination pagination, CancellationToken cancellationToken);
}
