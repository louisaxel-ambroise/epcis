using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.UseCases.TopLevelResources;

public interface IListEpcsHandler
{
    Task<IEnumerable<string>> ListEpcs(Pagination pagination, CancellationToken cancellationToken);
}
