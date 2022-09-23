namespace FasTnT.Application.UseCases.TopLevelResources;

public interface IListBizStepsHandler
{
    Task<IEnumerable<string>> ListBizSteps(CancellationToken cancellationToken);
}
