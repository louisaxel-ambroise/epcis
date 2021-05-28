using MediatR;

namespace FasTnT.Application.Queries.GetStandardVersion
{
    public record GetVendorVersionQuery : IRequest<GetVendorVersionResponse>
    {
    }
}
