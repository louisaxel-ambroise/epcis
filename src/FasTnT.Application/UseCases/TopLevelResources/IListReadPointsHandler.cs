namespace FasTnT.Application.UseCases.TopLevelResources;

public interface IListReadPointsHandler
{
    Task<IEnumerable<string>> ListReadPoints(CancellationToken cancellationToken);
}
