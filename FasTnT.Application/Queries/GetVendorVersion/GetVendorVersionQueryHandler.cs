using FasTnT.Domain;
using FasTnT.Domain.Queries.GetVendorVersion;
using FasTnT.Domain.Queries.Poll;
using MediatR;

namespace FasTnT.Application.Queries.GetVendorVersion;

public class GetVendorVersionQueryHandler : IRequestHandler<GetVendorVersionQuery, IEpcisResponse>
{
    public Task<IEpcisResponse> Handle(GetVendorVersionQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult<IEpcisResponse>(new GetVendorVersionResult(Constants.VendorVersion));
    }
}
