namespace FasTnT.Application.UseCases.ListTopLevelResources;

public interface IListBizLocationsHandler
{
    Task<IEnumerable<string>> ListBizLocations(CancellationToken cancellationToken);
}
