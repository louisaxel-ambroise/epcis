using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.UseCases.TopLevelResources;

public interface IListReadPoints
{
    Task<IEnumerable<string>> ListReadPoints(Pagination pagination, CancellationToken cancellationToken);
}
