namespace FasTnT.Application.UseCases.StandardQueries;

public interface IGetStandardQueryNamesHandler
{
    Task<IEnumerable<string>> GetQueryNamesAsync(CancellationToken cancellationToken);
}
