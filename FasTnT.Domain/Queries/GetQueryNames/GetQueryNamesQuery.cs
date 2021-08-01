using MediatR;

namespace FasTnT.Domain.Queries.GetQueryNames
{
    public record GetQueryNamesQuery : IRequest<GetQueryNamesResult>
    {
    }
}
