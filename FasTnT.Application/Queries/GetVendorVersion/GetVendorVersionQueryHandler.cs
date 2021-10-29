using FasTnT.Domain;
using FasTnT.Domain.Queries;
using MediatR;

namespace FasTnT.Application.Queries;

public class GetVendorVersionQueryHandler : IRequestHandler<GetVendorVersionQuery, GetVendorVersionResult>
{
    public Task<GetVendorVersionResult> Handle(GetVendorVersionQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult<GetVendorVersionResult>(new(Constants.VendorVersion));
    }
}
