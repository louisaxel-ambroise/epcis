using FasTnT.Domain.Queries.Poll;
using MediatR;

namespace FasTnT.Domain.Queries.GetSubscriptionIds;

public record GetSubscriptionIdsQuery : IRequest<IEpcisResponse>
{
    public string QueryName { get; init; }
}
