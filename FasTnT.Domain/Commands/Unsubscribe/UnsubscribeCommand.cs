using FasTnT.Domain.Queries.Poll;
using MediatR;

namespace FasTnT.Domain.Commands.Unsubscribe;

public class UnsubscribeCommand : IRequest<IEpcisResponse>
{
    public string SubscriptionId { get; init; }
}
