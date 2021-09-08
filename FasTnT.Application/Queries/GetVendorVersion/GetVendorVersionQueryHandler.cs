using FasTnT.Domain;
using FasTnT.Domain.Queries.GetVendorVersion;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FasTnT.Application.Queries.GetVendorVersion
{
    public class GetVendorVersionQueryHandler : IRequestHandler<GetVendorVersionQuery, GetVendorVersionResult>
    {
        public Task<GetVendorVersionResult> Handle(GetVendorVersionQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult<GetVendorVersionResult>(new(Constants.VendorVersion));
        }
    }
}