using FasTnT.Domain.Queries.GetStandardVersion;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

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
