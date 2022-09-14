using FasTnT.Application.Services;

namespace FasTnT.Application.UseCases.GetStandardQueryNames;

public class GetStandardQueryNamesHandler : IGetStandardQueryNamesHandler
{
    private readonly IEnumerable<IStandardQuery> _queries;

    public GetStandardQueryNamesHandler(IEnumerable<IStandardQuery> queries)
    {
        _queries = queries;
    }

    public Task<IEnumerable<string>> GetQueryNamesAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_queries.Select(x => x.Name));
    }
}
