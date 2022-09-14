namespace FasTnT.Application.UseCases.GetStandardQueryNames;

public interface IGetStandardQueryNamesHandler
{
    Task<IEnumerable<string>> GetQueryNamesAsync(CancellationToken cancellationToken);
}
