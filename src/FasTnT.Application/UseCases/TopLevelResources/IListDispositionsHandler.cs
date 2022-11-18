using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.UseCases.TopLevelResources;

public interface IListDispositionsHandler
{
    Task<IEnumerable<string>> ListDispositions(Pagination pagination, CancellationToken cancellationToken);
}
