using MediatR;

namespace FasTnT.Domain.Queries.GetStandardVersion
{
    public record GetVendorVersionQuery : IRequest<GetVendorVersionResponse>
    {
    }
}
