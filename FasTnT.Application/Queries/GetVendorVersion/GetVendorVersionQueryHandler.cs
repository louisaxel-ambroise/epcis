using FasTnT.Domain;
using FasTnT.Domain.Queries;
using MediatR;

namespace FasTnT.Application.Queries;

public class GetVendorVersionQueryHandler : IRequestHandler<GetVendorVersionQuery, IEpcisResponse>
{
    public Task<IEpcisResponse> Handle(GetVendorVersionQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult<IEpcisResponse>(new GetVendorVersionResult(Constants.VendorVersion));
    }
}
