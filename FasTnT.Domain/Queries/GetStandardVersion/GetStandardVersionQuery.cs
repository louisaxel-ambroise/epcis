using MediatR;

namespace FasTnT.Application.Queries.GetStandardVersion
{
    public record GetStandardVersionQuery : IRequest<GetStandardVersionResponse>
    {
    }
}
