using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.UseCases.TopLevelResources;

public interface IListEpcs
{
    Task<IEnumerable<string>> ListEpcs(Pagination pagination, CancellationToken cancellationToken);
}
