using FasTnT.Domain.Enumerations;

namespace FasTnT.Application.UseCases.ListTopLevelResources;

public interface IListEventTypesHandler
{
    Task<IEnumerable<EventType>> ListEventTypes(CancellationToken cancellationToken);
}
