namespace FasTnT.Application.UseCases.TopLevelResources;

public interface IListDispositionsHandler
{
    Task<IEnumerable<string>> ListDispositions(CancellationToken cancellationToken);
}
