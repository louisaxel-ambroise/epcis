using FasTnT.Domain;
using FasTnT.Domain.Queries;
using MediatR;

namespace FasTnT.Application.Queries;

public class GetStandardVersionQueryHandler : IRequestHandler<GetStandardVersionQuery, IEpcisResponse>
{
    public Task<IEpcisResponse> Handle(GetStandardVersionQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult<IEpcisResponse>(new GetStandardVersionResult(Constants.StandardVersion));
    }
}
