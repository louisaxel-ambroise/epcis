namespace FasTnT.Application.UseCases.ListTopLevelResources;

public interface IListDispositionsHandler
{
    Task<IEnumerable<string>> ListDispositions(CancellationToken cancellationToken);
}
