using FasTnT.Domain.Queries.GetStandardVersion;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Application.Queries.GetStandardVersion
{
    public class GetVendorVersionQueryHandler : IRequestHandler<GetVendorVersionQuery, GetVendorVersionResponse>
    {
        public Task<GetVendorVersionResponse> Handle(GetVendorVersionQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult<GetVendorVersionResponse>(new("0.5"));
        }
    }
}