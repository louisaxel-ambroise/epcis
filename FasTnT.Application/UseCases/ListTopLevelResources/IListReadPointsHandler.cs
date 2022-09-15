namespace FasTnT.Application.UseCases.ListTopLevelResources;

public interface IListReadPointsHandler
{
    Task<IEnumerable<string>> ListReadPoints(CancellationToken cancellationToken);
}
