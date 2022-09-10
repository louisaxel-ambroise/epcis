using FasTnT.Domain.Queries.GetQueryNames;
using FasTnT.Domain.Queries.Poll;
using MediatR;

namespace FasTnT.Application.Queries.GetQueryNames;

public class GetQueryNamesQueryHandler : IRequestHandler<GetQueryNamesQuery, IEpcisResponse>
{
    private readonly IEnumerable<Services.IEpcisQuery> _queries;

    public GetQueryNamesQueryHandler(IEnumerable<Services.IEpcisQuery> queries)
    {
        _queries = queries;
    }

    public Task<IEpcisResponse> Handle(GetQueryNamesQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult<IEpcisResponse>(new GetQueryNamesResult(_queries.Select(x => x.Name)));
    }
}
