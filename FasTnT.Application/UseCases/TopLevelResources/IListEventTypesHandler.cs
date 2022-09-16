namespace FasTnT.Application.UseCases.ListTopLevelResources;

public interface IListEventTypesHandler
{
    Task<IEnumerable<string>> ListEventTypes(CancellationToken cancellationToken);
}
