using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Application.Queries.GetStandardVersion
{
    public class GetStandardVersionQueryHandler : IQueryHandler<GetStandardVersionQuery, GetStandardVersionResponse>
    {
        public Task<GetStandardVersionResponse> Handle(GetStandardVersionQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult<GetStandardVersionResponse>(new() { Version = "1.2" });
        }
    }
}
