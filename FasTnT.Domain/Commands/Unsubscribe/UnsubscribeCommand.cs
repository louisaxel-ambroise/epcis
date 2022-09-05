using FasTnT.Domain.Queries;
using MediatR;

namespace FasTnT.Domain.Commands.Unsubscribe;

public class UnsubscribeCommand : IRequest<IEpcisResponse>
{
    public string SubscriptionId { get; init; }
}
