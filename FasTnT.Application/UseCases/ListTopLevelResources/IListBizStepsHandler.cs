namespace FasTnT.Application.UseCases.ListTopLevelResources;

public interface IListBizStepsHandler
{
    Task<IEnumerable<string>> ListBizSteps(CancellationToken cancellationToken);
}
