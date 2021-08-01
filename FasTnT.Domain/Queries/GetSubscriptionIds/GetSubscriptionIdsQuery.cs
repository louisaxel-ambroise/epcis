using MediatR;

namespace FasTnT.Domain.Queries.GetSubscriptionIds
{
    public record GetSubscriptionIdsQuery : IRequest<GetSubscriptionIdsResult>
    {
        public string QueryName { get; init; }
    }
}
