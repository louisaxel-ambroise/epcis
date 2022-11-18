using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.UseCases.TopLevelResources;

public interface IListEventTypesHandler
{
    Task<IEnumerable<string>> ListEventTypes(Pagination pagination, CancellationToken cancellationToken);
}
