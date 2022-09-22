namespace FasTnT.Application.UseCases.TopLevelResources;

public interface IListEventTypesHandler
{
    Task<IEnumerable<string>> ListEventTypes(CancellationToken cancellationToken);
}
