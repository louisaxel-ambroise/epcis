using MediatR;

namespace FasTnT.Domain.Commands.Unsubscribe;

public class UnsubscribeCommand : IRequest<UnsubscribeResult>
{
    public string SubscriptionId { get; init; }
}
