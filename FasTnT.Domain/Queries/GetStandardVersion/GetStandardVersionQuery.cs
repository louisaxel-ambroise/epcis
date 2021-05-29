using MediatR;

namespace FasTnT.Domain.Queries.GetStandardVersion
{
    public record GetStandardVersionQuery : IRequest<GetStandardVersionResponse>
    {
    }
}
