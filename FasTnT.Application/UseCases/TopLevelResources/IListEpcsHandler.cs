namespace FasTnT.Application.UseCases.ListTopLevelResources;

public interface IListEpcsHandler
{
    Task<IEnumerable<string>> ListEpcs(CancellationToken cancellationToken);
}
