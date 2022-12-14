using FasTnT.Domain.Model.Queries;

namespace FasTnT.Application.Services.DataSources.Utils;

public static class DataSourceExtensions
{
    public static T WithParameters<T>(this T dataSource, IEnumerable<QueryParameter> parameters) where T : IEpcisDataSource
    {
        dataSource.ApplyParameters(parameters);

        return dataSource;
    }
}
