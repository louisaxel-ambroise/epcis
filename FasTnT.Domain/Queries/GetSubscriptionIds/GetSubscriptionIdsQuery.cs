using MediatR;

namespace FasTnT.Domain.Queries;

public record GetSubscriptionIdsQuery : IRequest<GetSubscriptionIdsResult>
{
    public string QueryName { get; init; }
}
