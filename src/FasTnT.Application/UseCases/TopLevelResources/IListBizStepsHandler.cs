using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.UseCases.TopLevelResources;

public interface IListBizStepsHandler
{
    Task<IEnumerable<string>> ListBizSteps(Pagination pagination, CancellationToken cancellationToken);
}
