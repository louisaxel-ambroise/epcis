using MediatR;

namespace FasTnT.Domain.Queries.GetVendorVersion
{
    public record GetVendorVersionQuery : IRequest<GetVendorVersionResult>
    {
    }
}
