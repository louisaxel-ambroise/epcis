using FasTnT.Domain.Queries.GetStandardVersion;
using FasTnT.Domain.Queries.Poll;
using MediatR;

namespace FasTnT.Application.Queries.GetStandardVersion;

public class GetStandardVersionQueryHandler : IRequestHandler<GetStandardVersionQuery, IEpcisResponse>
{
    public Task<IEpcisResponse> Handle(GetStandardVersionQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult<IEpcisResponse>(new GetStandardVersionResult(request.StandardVersion));
    }
}
