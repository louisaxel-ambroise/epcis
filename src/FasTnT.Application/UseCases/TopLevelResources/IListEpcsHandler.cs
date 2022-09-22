namespace FasTnT.Application.UseCases.TopLevelResources;

public interface IListEpcsHandler
{
    Task<IEnumerable<string>> ListEpcs(CancellationToken cancellationToken);
}
