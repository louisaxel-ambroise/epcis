using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Services.DataSources;

public interface IEpcisDataSource
{
    void ApplyParameters(IEnumerable<QueryParameter> parameters);
    Task<QueryData> ExecuteAsync(CancellationToken cancellationToken);
}
