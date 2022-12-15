using FasTnT.Domain.Model.Queries;
using LinqKit;

namespace FasTnT.Application.Services.DataSources.Utils;

public static class DataSourceExtensions
{
    public static T WithParameters<T>(this T dataSource, IEnumerable<QueryParameter> parameters) where T : IEpcisDataSource
    {
        parameters.ForEach(dataSource.Apply);

        return dataSource;
    }
}
