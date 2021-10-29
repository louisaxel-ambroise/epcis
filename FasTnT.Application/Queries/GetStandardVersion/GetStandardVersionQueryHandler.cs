using FasTnT.Domain.Queries.GetStandardVersion;
using MediatR;

namespace FasTnT.Application.Queries.GetStandardVersion
{
    public class GetStandardVersionQueryHandler : IRequestHandler<GetStandardVersionQuery, GetStandardVersionResult>
    {
        public Task<GetStandardVersionResult> Handle(GetStandardVersionQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult<GetStandardVersionResult>(new("1.2"));
        }
    }
}
