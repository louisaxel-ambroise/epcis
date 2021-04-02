using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Application.Queries.GetStandardVersion
{
    public class GetVendorVersionQueryHandler : IQueryHandler<GetVendorVersionQuery, GetVendorVersionResponse>
    {
        public Task<GetVendorVersionResponse> Handle(GetVendorVersionQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new GetVendorVersionResponse { Version = "0.5" });
        }
    }
}