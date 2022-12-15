using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Services.DataSources;

public interface IEpcisDataSource
{
    void Apply(QueryParameter param);
    Task<QueryData> ExecuteAsync(CancellationToken cancellationToken);
}
