using MediatR;

namespace FasTnT.Domain.Queries;

public record GetSubscriptionIdsQuery : IRequest<IEpcisResponse>
{
    public string QueryName { get; init; }
}
