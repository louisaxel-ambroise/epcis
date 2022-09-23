namespace FasTnT.Application.UseCases.TopLevelResources;

public interface IListBizLocationsHandler
{
    Task<IEnumerable<string>> ListBizLocations(CancellationToken cancellationToken);
}
