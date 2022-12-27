using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.UseCases.TopLevelResources;

public interface IListBizSteps
{
    Task<IEnumerable<string>> ListBizSteps(Pagination pagination, CancellationToken cancellationToken);
}
